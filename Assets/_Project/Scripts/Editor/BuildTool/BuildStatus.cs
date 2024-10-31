using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace DaftAppleGames.Editor.BuildTool
{
    public enum BuildStage { Prototype, Alpha, Beta, EarlyAccess, Live }
    public enum BuildState { Success, SuccessWithWarnings, SuccessWithErrors, Failed }
    public enum DeployState { Success, Failed }

    public enum VersionIncrementType { MonthYear, Patch, Minor, Major }

    [Serializable]
    public class BuildStatus
    {
        [BoxGroup("Build Status")] public CustomVersion buildVersion;
        [BoxGroup("Build Status")] public CustomDateTime lastSuccessfulBuild;
        [BoxGroup("Build Status")] public CustomDateTime lastBuildAttempt;
        [BoxGroup("Build Status")] public BuildState lastBuildState;
        [BoxGroup("Build Status")] public string lastBuildLog;

        [BoxGroup("Deploy Status")] public CustomDateTime lastSuccessfulDeploy;
        [BoxGroup("Deploy Status")] public DeployState lastDeployState;
        [BoxGroup("Deploy Status")] public string lastDeployLog;

        public BuildStatus CopyBuildStatus()
        {
            BuildStatus newStatus = new BuildStatus
            {
                buildVersion = this.buildVersion,
                lastSuccessfulBuild = this.lastSuccessfulBuild,
                lastBuildAttempt = this.lastBuildAttempt,
                lastBuildState = this.lastBuildState,
                lastBuildLog = this.lastBuildLog,
                lastSuccessfulDeploy = this.lastSuccessfulDeploy,
                lastDeployState = this.lastDeployState,
                lastDeployLog = this.lastDeployLog
            };
            return newStatus;
        }

        [Serializable]
        public class CustomDateTime
        {
            public int day;
            public int month;
            public int year;
            public int hour;
            public int minute;
            public int second;

            /// <summary>
            /// Default constructor returns current date and time
            /// </summary>
            public CustomDateTime()
            {
                SetNow();
            }

            /// <summary>
            /// Set the given date and time to now
            /// </summary>
            public void SetNow()
            {
                DateTime now = DateTime.Now;
                day = now.Day;
                month = now.Month;
                year = now.Year;
                hour = now.Hour;
                minute = now.Minute;
                second = now.Second;
            }

            /// <summary>
            /// Returns a string version of the Date and Time
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"{day:D2}/{month:D2}/{year:D2} {hour:D2}:{minute:D2}:{second:D2}";
            }
        }

        [Serializable]
        public class CustomVersion
        {
            public int MajorVersionNumber
            {
                set
                {
                    majorVersionNumber = value;
                    PlayerSettings.bundleVersion = ToString();
                }
                get => majorVersionNumber;
            }

            public int MinorVersionNumber
            {
                set
                {
                    minorVersionNumber = value;
                    PlayerSettings.bundleVersion = ToString();
                }
                get => minorVersionNumber;
            }

            public int PatchVersionNumber
            {
                set
                {
                    patchVersionNumber = value;
                    PlayerSettings.bundleVersion = ToString();
                }
                get => patchVersionNumber;

            }

            public BuildStage BuildStage
            {
                set
                {
                    buildStage = value;
                    PlayerSettings.bundleVersion = ToString();
                }
                get => buildStage;
            }

            [SerializeField] private int majorVersionNumber;
            [SerializeField] private int minorVersionNumber;
            [SerializeField] private int patchVersionNumber;
            [SerializeField] private BuildStage buildStage;

            public void IncrementVersion(VersionIncrementType versionIncrementType)
            {
                switch (versionIncrementType)
                {
                    case VersionIncrementType.MonthYear:
                        SetVersionByDate();
                        break;

                    case VersionIncrementType.Patch:
                        IncrementPatchVersion();
                        break;

                    case VersionIncrementType.Minor:
                        IncrementMinorVersion();
                        break;

                    case VersionIncrementType.Major:
                        IncrementMajorVersion();
                        break;
                }
            }

            public CustomVersion(int majorVersionNumber, int minorVersionNumber, int patchVersionNumber,
                BuildStage buildStage)
            {
                MajorVersionNumber = majorVersionNumber;
                MinorVersionNumber = minorVersionNumber;
                PatchVersionNumber = patchVersionNumber;
                BuildStage = buildStage;
            }

            /// <summary>
            /// Sets the version number based on today's date, and sets the build stage
            /// </summary>
            public void SetVersionByDate(BuildStage newBuildStage)
            {
                SetVersionByDate();
                BuildStage = newBuildStage;
            }

            /// <summary>
            /// Sets the version number based on today's date, incrementing the patch version at the beginning of each month
            /// </summary>
            public void SetVersionByDate()
            {
                DateTime now = DateTime.Now;

                MajorVersionNumber = now.Year;
                if (now.Month != MinorVersionNumber)
                {
                    PatchVersionNumber = 1;
                    MinorVersionNumber = now.Month;
                }
                else
                {
                    PatchVersionNumber++;
                }
            }

            /// <summary>
            /// Returns a string representation of the build version.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"{MajorVersionNumber}.{MinorVersionNumber}.{PatchVersionNumber} ({BuildStage.ToString()})";
            }

            public string GetDateFormattedVersionString()
            {
                return $"{MajorVersionNumber}.{MinorVersionNumber:D2}.{PatchVersionNumber} ({BuildStage.ToString()})";
            }

            public string GetFullVersionString()
            {
                return $"{MajorVersionNumber}.{MinorVersionNumber:D2}.{PatchVersionNumber} ({BuildStage.ToString()})";
            }


            /// <summary>
            /// Increments the Major version
            /// </summary>
            public void IncrementMajorVersion()
            {
                MajorVersionNumber++;
            }

            /// <summary>
            /// Increments the Minor version
            /// </summary>
            public void IncrementMinorVersion()
            {
                MinorVersionNumber++;
            }

            /// <summary>
            /// Increments the Patch version
            /// </summary>
            public void IncrementPatchVersion()
            {
                PatchVersionNumber++;
            }
        }
    }
}