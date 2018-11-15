namespace MinVer
{
    using System;

    internal class Logger : ILogger
    {
        private readonly Verbosity level;

        public Logger(Verbosity level) => this.level = level;

        public bool IsDebugEnabled => this.level >= Verbosity.Detailed;

        public void Debug(Func<string> createMessage)
        {
            if (this.level >= Verbosity.Detailed)
            {
                Message(createMessage());
            }
        }

        public void Debug(string message)
        {
            if (this.level >= Verbosity.Detailed)
            {
                Message(message);
            }
        }

        public void Info(string message)
        {
            if (this.level >= Verbosity.Normal)
            {
                Message(message);
            }
        }

        public static void WarnInvalidRepoPath(string path, Version version) =>
            Warn(1001, $"'{path}' is not a valid repository or working directory. Using default version: {version}");

        public static void ErrorInvalidRepoPath(string path) =>
            Error(1002, $"Invalid repository path '{path}'. Directory does not exist.");

        public static void ErrorInvalidMajorMinorRange(string majorMinor) =>
            Error(1003, $"Invalid MAJOR.MINOR range '{majorMinor}'.");

        public static void ErrorInvalidVerbosityLevel(string verbosity) =>
            Error(1004, $"Invalid verbosity level '{verbosity}'. Valid levels are quiet, minimal, normal, detailed, and diagnostic");

        private static void Warn(int code, string message) => Message($"warning MINVER{code:D4} : {message}");

        private static void Error(int code, string message) => Message($"error MINVER{code:D4} : {message}");

        private static void Message(string message) => Console.Error.WriteLine($"MinVer: {message}");
    }
}