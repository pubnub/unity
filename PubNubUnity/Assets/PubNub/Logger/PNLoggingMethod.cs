using System;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    #region "Logging and error codes"
    public class PNLoggingMethod
    {
        private static int logLevel;

        public static Level LogLevel {
            get {
                return (Level)logLevel;
            }
            set {
                logLevel = (int)value;
            }
        }

        public enum Level
        {
            Off,
            Error,
            Info,
            Verbose,
            Warning
        }

        public static bool LevelError {
            get {
                return (int)LogLevel >= 1;
            }
        }

        public static bool LevelInfo {
            get {
                return (int)LogLevel >= 2;
            }
        }

        public static bool LevelVerbose {
            get {
                return (int)LogLevel >= 3;
            }
        }

        public static bool LevelWarning {
            get {
                return (int)LogLevel >= 4;
            }
        }

        public PNLogVerbosity PNLogVerb {get; set;}

        public PNLoggingMethod(PNLogVerbosity pnLogVerbosity){
            PNLogVerb = pnLogVerbosity;
        }

        //write is kept for future improvements in logging, can be used to add levels to logging
        public void WriteToLog (string logText, bool write)
        {
            if (PNLogVerb.Equals(PNLogVerbosity.BODY)) {
                UnityEngine.Debug.Log (string.Format("\n{0} {1}: {2} \n", DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToShortTimeString(), logText));
            }
        }
    }
    #endregion
}

