import os
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
from scipy.stats import ttest_ind
import statsmodels.api as sm
from statsmodels.formula.api import ols
from statsmodels.stats.multicomp import pairwise_tukeyhsd

data_directory = "C:/School/Erez_Lab/final_project/"
combined_df = pd.read_csv(os.path.join(data_directory,'combined_participants_data.csv'))
clean_combined_df = combined_df.drop(columns=['experiment','ppid','session_num','trial_num','block_num','trial_num_in_block','start_time','end_time','curr_pos','eyetracking_eye_tracking_location_0'])

side_map = {'near_right': 1, 'near_left': 2, 'far_right': 3, 'far_left': 4}
depth_map = {'near': 1, 'far': 2}
clutter_map = {'without_clutter': 0, 'with_clutter': 1}

clean_combined_df['side'] = clean_combined_df.dropna()['side'].replace(side_map)
clean_combined_df['depth'] = clean_combined_df.dropna()['depth'].replace(depth_map)
clean_combined_df['clutter'] = clean_combined_df.dropna()['clutter'].replace(clutter_map)

clean_combined_df['timeout'] = clean_combined_df.dropna()['timeout'].astype(int)
clean_combined_df['pressed'] = clean_combined_df.dropna()['pressed'].astype(int)

# save for later
clean_combined_df.to_csv(data_directory + 'clean_combined_df.csv', index=False)


# Descriptive statistics
descriptive_stats = clean_combined_df.describe()
with pd.option_context('display.max_rows', None, 'display.max_columns', None):  # more options can be specified also
    print(descriptive_stats)
descriptive_stats.to_csv(data_directory + 'descriptive_stats.csv')

print ("------------------------------------------------------")

# Group by condition (clutter vs no clutter) and calculate the mean
grouped_stats = clean_combined_df.groupby('clutter').mean(numeric_only=True)
with pd.option_context('display.max_rows', None, 'display.max_columns', None):  # more options can be specified also
    print(grouped_stats)
grouped_stats.to_csv(data_directory + 'grouped_stats.csv')

print ("------------------------------------------------------")

# Boxplot: Trial Duration by Clutter and Side
plt.figure(figsize=(12, 6))
sns.boxplot(x='side', y='trial_duration', hue='clutter', data=combined_df)
plt.title('Trial Duration by Object Location (side) and Clutter Presence')
plt.xlabel('Object Location (side)')
plt.ylabel('Trial Duration (seconds)')
plt.legend(title='Clutter')
plt.show()

# Boxplot: Total Saccades by Clutter and Side
plt.figure(figsize=(12, 6))
sns.boxplot(x='side', y='Total_Saccades', hue='clutter', data=combined_df)
plt.title('Total Saccades by Object Location (side) and Clutter Presence')
plt.xlabel('Object Location (side)')
plt.ylabel('Total Saccades')
plt.legend(title='Clutter')
plt.show()

# Pairplot: Relationships between variables with hue as Clutter
sns.pairplot(combined_df, hue='clutter', vars=['trial_duration', 'Total_Saccades', 'Mean_Speed', 'Mean_Acceleration'], diag_kind='kde')
plt.suptitle('Pairplot of Eye Movement Metrics by Clutter Presence', y=1.02)
plt.show()

# Heatmap for correlation between numeric features
plt.figure(figsize=(12, 8))
sns.heatmap(clean_combined_df.corr(), annot=True, cmap='coolwarm')
plt.title('Correlation Matrix')
plt.show()

# T-tests for Clutter Presence

# Grouping the data by clutter presence
cluttered = clean_combined_df[clean_combined_df['clutter'] == 1]
not_cluttered = clean_combined_df[clean_combined_df['clutter'] == 0]

# Trial Duration t-test
t_stat, p_val = ttest_ind(cluttered['trial_duration'], not_cluttered['trial_duration'])
print(f"T-test for Trial Duration: t-statistic={t_stat}, p-value={p_val}")

# Total Saccades t-test
t_stat, p_val = ttest_ind(cluttered['Total_Saccades'], not_cluttered['Total_Saccades'])
print(f"T-test for Total Saccades: t-statistic={t_stat}, p-value={p_val}")

# ANOVA for Object Location

# ANOVA: Trial Duration by Side
model = ols('trial_duration ~ C(side)', data=clean_combined_df).fit()
anova_table = sm.stats.anova_lm(model, typ=2)
print(f"ANOVA for Trial Duration by Side:\n{anova_table}\n")

# ANOVA: Total Saccades by Side
model = ols('Total_Saccades ~ C(side)', data=clean_combined_df).fit()
anova_table = sm.stats.anova_lm(model, typ=2)
print(f"ANOVA for Total Saccades by Side:\n{anova_table}\n")

#Post-Hoc Analysis:

# Tukey's HSD for Trial Duration
tukey_duration = pairwise_tukeyhsd(endog=clean_combined_df['trial_duration'], groups=clean_combined_df['side'], alpha=0.05)
print(tukey_duration)

# Tukey's HSD for Total Saccades
tukey_saccades = pairwise_tukeyhsd(endog=clean_combined_df['Total_Saccades'], groups=clean_combined_df['side'], alpha=0.05)
print(tukey_saccades)

# Boxplot for Trial Duration by Side
plt.figure(figsize=(10, 6))
sns.boxplot(x='side', y='trial_duration', data=clean_combined_df)
plt.title('Trial Duration by Side')
plt.show()

# Boxplot for Total Saccades by Side
plt.figure(figsize=(10, 6))
sns.boxplot(x='side', y='Total_Saccades', data=clean_combined_df)
plt.title('Total Saccades by Side')
plt.show()



# T-tests for Each Side with Clutter
for side in clean_combined_df['side'].unique():
    side_group = clean_combined_df[clean_combined_df['side'] == side]
    # to find the name of the side to print it more nicely
    for k,v in side_map.items():
        if v == side:
            _side = k

    # Trial Duration t-test for each side
    t_stat, p_val = ttest_ind(side_group[side_group['clutter'] == 1]['trial_duration'],
                              side_group[side_group['clutter'] == 0]['trial_duration'])
    print(f"T-test for Trial Duration in {_side}: t-statistic={t_stat}, p-value={p_val}")

    # Total Saccades t-test for each side
    t_stat, p_val = ttest_ind(side_group[side_group['clutter'] == 1]['Total_Saccades'],
                              side_group[side_group['clutter'] == 0]['Total_Saccades'])
    print(f"T-test for Total Saccades in {_side}: t-statistic={t_stat}, p-value={p_val}")
