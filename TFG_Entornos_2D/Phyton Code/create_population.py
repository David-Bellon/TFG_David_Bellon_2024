import torch
from architecture import CarNet

for i in range(20):
    torch.manual_seed(42)
    torch.cuda.manual_seed(42)
    model = CarNet()
    x = torch.rand(1, 5)
    out = model(x)
    torch.onnx.export(model, x, f"C:/Users/dadbc/TFG 2D/Assets/Resources/Scenario1/individual_{i}.onnx")
    torch.save(model, f"neural_models/individual_{i}.pt")

