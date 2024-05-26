import pandas as pd
import torch
import random
import numpy as np
from architecture import CarNet
from flask import Flask, request

app = Flask(__name__)
aux = 0

@app.route("/home", methods=["POST"])
def mainTask():
    global aux
    mutate_proba = 8
    all_scores = request.form
    max_score1 = 0
    max_score2 = 0
    file_1 = 0
    file_2 = 0
    for (id, score) in all_scores.items():
        score = float(score.replace(",", "."))
        if (score > max_score1 or score > max_score2):
            if (score > max_score1):
                if (max_score1 > max_score2):
                    max_score2 = max_score1
                    file_2 = file_1
                max_score1 = score
                file_1 = id
            else:
                max_score2 = score
                file_2 = id
    print(list(all_scores.items()))
    print(file_1)
    print(file_2)
    parent_1 = torch.load(f"neural_models/individual_{file_1}.pt")
    parent_2 = torch.load(f"neural_models/individual_{file_2}.pt")

    x = torch.rand(1, 5)

    parent_1_gens = parent_1.state_dict()
    parent_2_gens = parent_2.state_dict()
    for number in range(20):
        #Breed new gen and muatate
        children_gens = {a: torch.zeros(parent_1_gens[a].size()) for a in parent_1_gens.keys()}

        for i in children_gens.keys():
            parent_1_data = parent_1_gens[i].flatten()
            parent_2_data = parent_2_gens[i].flatten()
            new_gens = children_gens[i].flatten()
            split = random.randint(0, len(parent_1_data) - 1)
            new_gens[0:split] = parent_1_data[0:split]
            new_gens[split:] = parent_2_data[split:]
            if random.randint(1, 10) <= mutate_proba:
                mutate = random.randint(0, len(parent_1_data) - 1)
                new_gens[mutate] = np.random.normal()
                children_gens[i] = new_gens.unflatten(0, children_gens[i].size())

        children = CarNet()
        children.load_state_dict(children_gens)
        torch.save(children, f"neural_models/individual_{number}.pt")

        torch.onnx.export(children, x, f"C:/Users/dadbc/TFG 2D/Assets/Resources/Scenario1/individual_{number}.onnx")

    df = pd.DataFrame(all_scores.items(), columns=["CarNumber", "Score"])
    df.to_csv(f"Data/gen_{aux}.csv", index=False)
    aux = aux + 1
    print(mutate_proba)
    return {"data": "Breeder Completed"}, 200


@app.route("/scores", methods=["POST"])
def tata():
    all_scores = request.form
    print(all_scores.keys())
    return {"data": "hey"}, 200


if __name__ == "__main__":
    app.run()