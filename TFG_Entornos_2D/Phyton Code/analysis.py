import pandas as pd
import matplotlib.pyplot as plt
import os
import numpy as np
from PIL import Image

def get_images():
    for i in range(len(os.listdir("Data"))):
        file = f"Data/gen_{i}.csv"
        df = pd.read_csv(file)
        data = df["Score"].apply(lambda x: x[:5].replace(",", ".")).astype(float)
        plt.hist(data)
        plt.title(f"Gen {i}")
        plt.xticks(np.arange(0, 33, 3))
        plt.savefig(f"Generation Distribution/dist_gen_{i}")
        #plt.show()
    plt.show()

def get_stats():
    total_mean = []
    max_score = []
    std_generation = []
    for i in range(len(os.listdir("Data"))):
        file = f"Data/gen_{i}.csv"
        df = pd.read_csv(file)
        data = df["Score"].apply(lambda x: x[:5].replace(",", ".")).astype(float)
        total_mean.append(data.mean())
        max_score.append(data.max())
        std_generation.append(data.std())
    # Plot mean of each generation
    plt.plot(total_mean)
    plt.title("Mean Score of each Generation")
    plt.xlabel("Generation")
    plt.ylabel("Mean Value")
    plt.savefig("tmp/mean5.png")
    plt.show()
    print(f"Best Generation score: {max(total_mean)}")
    print(f"Last Generation score: {total_mean[-1]}")


    # Plot max score of each generation
    plt.plot(max_score)
    plt.title("Max Score of each Generation")
    plt.xlabel("Generation")
    plt.ylabel("Max Value")
    plt.savefig("tmp/max_score5.png")
    plt.show()
    print(f"Max Score: {max(max_score)}")

    # Plot std score of each generation
    plt.plot(std_generation)
    plt.title("Std Score of each Generation")
    plt.xlabel("Generation")
    plt.ylabel("Std Value")
    plt.savefig("tmp/std5.png")
    plt.show()

    fig, axes = plt.subplots(nrows=2, ncols=2)

    image1 = Image.open("Generation Distribution/dist_gen_0.png")
    image2 = Image.open("Generation Distribution/dist_gen_25.png")
    image3 = Image.open("Generation Distribution/dist_gen_50.png")
    image4 = Image.open("Generation Distribution/dist_gen_100.png")

    # Plot each image on a separate subplot
    axes[0, 0].imshow(image1)
    axes[0, 0].set_title('Distribución Generación 0')
    axes[0, 1].imshow(image2)
    axes[0, 1].set_title('Distribución Generación 25')
    axes[1, 0].imshow(image3)
    axes[1, 0].set_title('Distribución Generación 50')
    axes[1, 1].imshow(image4)
    axes[1, 1].set_title('Distribución Generación 100')
    for ax in axes.flatten():
        ax.axis('off')

    plt.tight_layout()
    plt.savefig("tmp/Collage5.png")


def images_to_gif():
    images = []
    for i in range(len(os.listdir("Generation Distribution"))):
        img = Image.open(f"Generation Distribution/dist_gen_{i}.png")
        images.append(img)

    images[0].save("output5.gif", save_all=True, append_images=images[1:], loop=0, duration=500)


if __name__ == "__main__":
    get_images()
    get_stats()
    images_to_gif()
