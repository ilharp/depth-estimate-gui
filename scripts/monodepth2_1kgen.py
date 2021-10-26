import sys
from pathlib import Path

import matplotlib as mpl
import matplotlib.cm as cm
import numpy as np
import PIL.Image as pil
import torch
from torchvision import transforms

import networks


def run():
    image_name = sys.argv[1]
    colormap_name = sys.argv[2]

    if torch.cuda.is_available():
        device = torch.device("cuda")
    else:
        device = torch.device("cpu")

    root = Path(sys.path[0]).resolve()

    model_path = root / "models"
    encoder_path = str(model_path / "encoder.pth")
    depth_decoder_path = str(model_path / "depth.pth")

    # LOADING PRETRAINED MODEL
    encoder = networks.ResnetEncoder(18, False)
    loaded_dict_enc = torch.load(encoder_path, map_location=device)

    # extract the height and width of image that this model was trained with
    feed_height = loaded_dict_enc['height']
    feed_width = loaded_dict_enc['width']
    filtered_dict_enc = {
        k: v for k, v in loaded_dict_enc.items() if k in encoder.state_dict()}
    encoder.load_state_dict(filtered_dict_enc)
    encoder.to(device)
    encoder.eval()

    depth_decoder = networks.DepthDecoder(
        num_ch_enc=encoder.num_ch_enc, scales=range(4))

    loaded_dict = torch.load(depth_decoder_path, map_location=device)
    depth_decoder.load_state_dict(loaded_dict)

    depth_decoder.to(device)
    depth_decoder.eval()

    data_path = root.parent.parent / "data"
    input_path = str(data_path / "inputs" / image_name)
    output_path = str(data_path / "outputs" / image_name)

    with torch.no_grad():
        # Load image and preprocess
        input_image = pil.open(input_path).convert('RGB')
        original_width, original_height = input_image.size
        input_image = input_image.resize(
            (feed_width, feed_height), pil.LANCZOS)
        input_image = transforms.ToTensor()(input_image).unsqueeze(0)

        # PREDICTION
        input_image = input_image.to(device)
        features = encoder(input_image)
        outputs = depth_decoder(features)

        disp = outputs[("disp", 0)]
        disp_resized = torch.nn.functional.interpolate(
            disp, (original_height, original_width), mode="bilinear", align_corners=False)

        # Saving colormapped depth image
        prediction = disp_resized.squeeze().cpu().numpy()
        vmax = np.percentile(prediction, 95)
        normalizer = mpl.colors.Normalize(
            vmin=prediction.min(), vmax=vmax)
        mapper = cm.ScalarMappable(norm=normalizer, cmap=colormap_name)
        colormapped_im = (mapper.to_rgba(prediction)[
                          :, :, :3] * 255).astype(np.uint8)
        im = pil.fromarray(colormapped_im)
        im.save(output_path)

    print('Done')


if __name__ == '__main__':
    run()
