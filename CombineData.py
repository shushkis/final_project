import os
import pandas as pd
from glob import glob


data_directory = "C:/School/Erez_Lab/final_project/"

# Initialize lists to collect data from all participants
all_participants_data = []

# Loop through each participant's data
for participant_file in glob(data_directory + '/eye_gaze_exp/*/*/trial_results.csv'):
    if participant_file.endswith('.csv'):
        df = pd.read_csv(os.path.join(data_directory, participant_file))

        saccades_list = []
        speed_list = []
        acceleration_list = []

        # Loop over each trial in the participant's dataset
        for index, row in df.iterrows():
            # Load the eye-tracking data for this specific trial
            eye_tracking_path = row['eyetracking_eye_tracking_location_0']
            eye_tracking_path = os.path.join(data_directory, eye_tracking_path)
            eye_tracking_data = pd.read_csv(eye_tracking_path)

            # Summarize the eye-tracking data
            total_saccades = eye_tracking_data['saccade'].sum()  # Total number of saccades
            mean_speed = eye_tracking_data['speed'].mean()  # Mean speed of eye movement
            mean_acceleration = eye_tracking_data['acceleration'].mean()  # Mean acceleration of eye movement

            # Append the summarized data to the lists
            saccades_list.append(total_saccades)
            speed_list.append(mean_speed)
            acceleration_list.append(mean_acceleration)

        # Add the summarized eye-tracking data back to the participant's DataFrame
        df['Total_Saccades'] = saccades_list
        df['Mean_Speed'] = speed_list
        df['Mean_Acceleration'] = acceleration_list

        # Create some basic features for analysis
        df['trial_duration'] = df['end_time'] - df['start_time']  # Duration of each trial
        df['clutter'] = df['block_num'].apply(lambda x: 'with_clutter' if x % 2 == 0 else 'without_clutter')

        participant_id = os.path.splitext(participant_file)[0]
        df['ppid'] = participant_id

        all_participants_data.append(df)

# Combine all participants' data into a single DataFrame
combined_df = pd.concat(all_participants_data, ignore_index=True)

participant_summary = combined_df.groupby('ppid').agg({
    'trial_duration': ['mean', 'std'],  # Mean and Std of trial duration
    'Total_Saccades': ['mean', 'std'],  # Mean and Std of total saccades
    'Mean_Speed': ['mean', 'std'],  # Mean and Std of eye speed
    'Mean_Acceleration': ['mean', 'std']  # Mean and Std of eye acceleration
}).reset_index()

participant_summary.columns = ['_'.join(col).strip('_') for col in participant_summary.columns.values]

combined_df.to_csv(data_directory + 'combined_participants_data.csv', index=False)

participant_summary.to_csv(data_directory + 'participant_summary.csv', index=False)

print(participant_summary.head())
