import sys
import torch
import utils
import cv2
from pathlib import Path

import matplotlib as mpl
import matplotlib.cm as cm
import numpy as np
import PIL.Image as pil
from torchvision.transforms import Compose
from midas.dpt_depth import DPTDepthModel
from midas.transforms import Resize, NormalizeImage, PrepareForNet


def run():
    image_name = sys.argv[1]
    colormap_name = sys.argv[2]

    if torch.cuda.is_available():
        device = torch.device("cuda")
    else:
        device = torch.device("cpu")

    root = Path(sys.path[0]).resolve()

    model = DPTDepthModel(
        path=str(root / "weights" / "dpt_large.pt"),
        backbone="vitl16_384",
        non_negative=True,
    )
    net_w, net_h = 384, 384
    resize_mode = "minimal"
    normalization = NormalizeImage(
        mean=[0.5, 0.5, 0.5], std=[0.5, 0.5, 0.5])

    transform = Compose(
        [
            Resize(
                net_w,
                net_h,
                resize_target=None,
                keep_aspect_ratio=True,
                ensure_multiple_of=32,
                resize_method=resize_mode,
                image_interpolation_method=cv2.INTER_CUBIC,
            ),
            normalization,
            PrepareForNet(),
        ]
    )

    model.eval()
    model.to(device)

    data_path = root.parent.parent / "data"
    input_path = str(data_path / "inputs" / image_name)
    output_path = str(data_path / "outputs" / image_name)

    img = utils.read_image(input_path)
    img_input = transform({"image": img})["image"]

    with torch.no_grad():
        sample = torch.from_numpy(img_input).to(device).unsqueeze(0)
        prediction = model.forward(sample)
        prediction = (
            torch.nn.functional.interpolate(
                prediction.unsqueeze(1),
                size=img.shape[:2],
                mode="bicubic",
                align_corners=False,
            )
            .squeeze()
            .cpu()
            .numpy()
        )

    vmax = np.percentile(prediction, 95)
    normalizer = mpl.colors.Normalize(
        vmin=prediction.min(), vmax=vmax)
    mapper = cm.ScalarMappable(norm=normalizer, cmap=colormap_name)
    colormapped_im = (mapper.to_rgba(prediction)[
                      :, :, :3] * 255).astype(np.uint8)
    im = pil.fromarray(colormapped_im)
    im.save(output_path)

    print("Done")


if __name__ == "__main__":
    run()
