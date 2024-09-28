import pandas as pd
import matplotlib.pyplot as plt


file_path = r'C:\School\Erez_Lab\final_project\eye_gaze_exp\quiet-7247b148-9ba2-4b51-b471-b5edf6cf36ae\S001\trackers\eyetracking_eye_tracking_T005.csv'
data = pd.read_csv(file_path)

# Plot gaze directions over time
fig = plt.figure(figsize=(10, 6))
#ax = fig.add_subplot(111, projection='3d')


# ax.plot(data['gaze_direction_global_x'], data['gaze_direction_global_y'], data['gaze_direction_global_z'], label='Gaze Direction', color='blue')
plt.plot(data['gaze_direction_global_x'], data['gaze_direction_global_z'], label='Gaze Direction', color='blue')

# Mark the saccades
saccades = data[data['saccade'] == 1]  # Assuming saccade is indicated as 1
#ax.scatter(saccades['gaze_direction_global_x'], saccades['gaze_direction_global_y'], saccades['gaze_direction_global_z'], color='red', label='Saccade')
plt.scatter(saccades['gaze_direction_global_x'], saccades['gaze_direction_global_z'], color='red', label='Saccade',
            zorder=5)

# Adding labels and title
# ax.set_xlabel('Gaze Direction X')
# ax.set_ylabel('Gaze Direction Y')
# # ax.set_zlabel('Gaze Direction Z')
# plt.title('Gaze Directions with Saccades')
# plt.legend()

plt.xlabel('Gaze Direction X')
plt.ylabel('Gaze Direction Z')
plt.title('2D Gaze Directions with Saccades')
plt.legend()

# Show the plot
plt.show()