import numpy as np
import pandas as pd

data_directory = "C:/School/Erez_Lab/final_project/"


# Parameters based on original data
n_samples = 100000  # Number of samples to generate
ppid_range = [x for x in range (50)]  # Participant IDs
side_categories = ['far_left', 'far_right', 'near_left', 'near_right']
clutter_present = [0, 1]  # 0 for no clutter, 1 for clutter

# Mean and standard deviation for generating synthetic data based on observed means
trial_duration_means = {
    'far_left': 2.0,
    'far_right': 2.1,
    'near_left': 2.5,
    'near_right': 2.2
}

total_saccades_means = {
    'far_left': 10.0,
    'far_right': 11.0,
    'near_left': 14.0,
    'near_right': 13.0
}

# Standard deviations for duration and saccades
duration_std = 0.5
saccades_std = 3.0

# Generate synthetic data
np.random.seed(42)  # For reproducibility

data = []
for _ in range(n_samples):
    ppid = np.random.choice(ppid_range)
    side = np.random.choice(side_categories)
    clutter = np.random.choice(clutter_present)

    # Generate trial duration
    trial_duration = np.random.normal(trial_duration_means[side], duration_std)

    # Generate total saccades
    total_saccades = np.random.normal(total_saccades_means[side], saccades_std)

    # Simulate logistic regression target: pressed
    pressed_probability = 1 / (1 + np.exp(-(trial_duration - 2.0) + (total_saccades - 10) / 2))
    pressed = np.random.binomial(1, pressed_probability)

    depth = 'far' if side == 'far_left' or side == 'far_right' else 'near'

    data.append([ppid, depth, clutter, trial_duration, total_saccades, pressed])

# Create DataFrame
df_synthetic = pd.DataFrame(data, columns=['ppid', 'depth', 'clutter', 'trial_duration', 'Total_Saccades', 'pressed'])

# Save for later
df_synthetic.to_csv(data_directory + 'synthetic_dataset.csv', index=False)

print(df_synthetic.head())
