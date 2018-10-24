using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Logging
{
    /// <summary>
    /// Represents a log message type
    /// </summary>
    public enum LogMessageType
    {
        /// <summary>
        /// Parallel to Debug.LogError
        /// </summary>
        Error,
        /// <summary>
        /// Parallel to Debug.LogWarning
        /// </summary>
        Warning,
        /// <summary>
        /// Parallel to Debug.Log
        /// </summary>
        Log
    }

    public static class MessageBuilder
    {
        public static string Trace(string message)
        {
#if STACK_TRACE
            StackTrace myTrace = new StackTrace(true);
            StackFrame myFrame = myTrace.GetFrame(1);
            var method = myFrame.GetMethod();

            var functionName = method.Name;
            var className = method.DeclaringType.Name;
#else
			className = "CLASS";
			functionName = "FUNCTION";
#endif
            return className + "::" + functionName + "() >> " + message;
        }

        public static string Trace()
        {
#if STACK_TRACE
            StackTrace myTrace = new StackTrace(true);
            StackFrame myFrame = myTrace.GetFrame(1);
            var method = myFrame.GetMethod();

            var functionName = method.Name;
            var className = method.DeclaringType.Name;
#else
			className = "CLASS";
			functionName = "FUNCTION";
#endif
            return className + "::" + functionName + "()";
        }
    }

    /// <summary>
    /// Contains some functions for log handling
    /// </summary>
    public static class LogUtils
    {
        /// <summary>
        /// The logger to use
        /// </summary>
        public static ILogger Logger
        {
            get
            {
                if (_logger == null)
                    throw new Exception("LogUtils::Logger.get() : Logger is null");

                return _logger;
            }
            set => _logger = value;
        }

        public static bool IsLoggerSet => _logger != null;

        /// <summary>
        /// The logger to use
        /// </summary>
        private static ILogger _logger;

        /// <summary>
        /// Print to the console the message
        /// </summary>
        /// <remarks>Only happens when LOG symbol has been declared</remarks>
        /// <param name="message">The message to log</param>
        /// <param name="messageType">The type of the log</param>
        /// <param name="context">An optional context (when the user will click on the message in the console it will lead it to the context)</param>
        public static void Log(string message, LogMessageType messageType = LogMessageType.Log, object context = null)
        {
#if LOG
            Logger.Log(message, messageType, context);
#endif
        }

        /// <summary>
        /// Used for validating a condition, if not met, it will print to the console the message and pause the editor.
        /// </summary>
        /// <remarks>Only happens when ASSERT symbol has been declared</remarks>
        /// <param name="condition">The condition that needs to be met</param>
        /// <param name="message">The message to print to the console in case not met</param>
        /// <param name="context">An optional context (when the user will click on the message in the console it will lead it to the context)</param>
        public static void Assert<T>(bool condition, T message, object context = null)
            where T : struct, ILogMessage
        {
#if ASSERT
            if (condition)
                return;

            // Condition not met, pause the editor and assert
            Break(message.ToString(), context);
#endif
        }

        /// <summary>
        /// Used for validating a condition, if not met, it will print to the console the message and pause the editor.
        /// Optimized version.
        /// </summary>
        /// <remarks>Only happens when ASSERT symbol has been declared</remarks>
        /// <param name="condition">The condition that needs to be met</param>
        /// <param name="message">a log message to print to the console in case not met</param>
        /// <param name="context">An optional context (when the user will click on the message in the console it will lead it to the context)</param>
        public static void Assert(bool condition, string message, object context = null)
        {
#if ASSERT
            if (condition)
                return;

            // Condition not met, pause the editor and assert
            Break(message, context);
#endif
        }

        /// <summary>
        /// Used for breaking; halting the editor and printing a message
        /// </summary>
        /// <param name="message">The message to print to the console in case not met</param>
        /// <param name="context">An optional context (when the user will click on the message in the console it will lead it to the context)</param>
        public static void Break(string message, object context = null)
        {
            Logger.Break();
            throw new AssertException(message, context);
        }
    }

    /// <summary>
    /// Used for deciding which logger will log the messages and breaks
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="message">The message contents</param>
        /// <param name="type">The type of the message</param>
        /// <param name="context">The context of the message</param>
        void Log(string message, LogMessageType type = LogMessageType.Log, object context = null);

        /// <summary>
        /// Breaks the engine
        /// </summary>
        void Break();
    }

    /// <summary>
    /// The format of the trace messages
    /// </summary>
    public struct TraceMessage : ILogMessage
    {
        /// <summary>
        /// The message itself
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of TraceMessage
        /// </summary>
        /// <param name="message">The message itself</param>
        public TraceMessage(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Parse a trace log message to get the full message to print in the log
        /// </summary>
        public override string ToString()
        {
#if STACK_TRACE
            StackTrace myTrace = new StackTrace(true);
            StackFrame myFrame = myTrace.GetFrame(2);
            var method = myFrame.GetMethod();

            var functionName = method.Name;
            var className = method.DeclaringType.Name;
#else
			className = "CLASS";
			functionName = "FUNCTION";
#endif

            if (Message != null)
                return className + "::" + functionName + "() >> " + Message;

            return className + "::" + functionName + "()";
        }
    }

    /// <summary>
    /// Represents a log message to a console
    /// </summary>
    public interface ILogMessage
    {
        /// <summary>
        /// Returns the string format of this log message type
        /// </summary>
        string ToString();
    }

    /// <summary>
    /// Wraps an exception for assertion. Prints the message to the console
    /// </summary>
    public class AssertException : Exception
    {
        public AssertException(ILogMessage message, object context = null) : base("An assert has been detected")
        {
            LogUtils.Log(message.ToString(), LogMessageType.Error, context);
        }

        public AssertException(string message, object context = null) : base("An assert has been detected")
        {
            LogUtils.Log(message, LogMessageType.Error, context);
        }
    }

    /// <summary>
    /// The format of a normal log message (Containing only the raw string)
    /// </summary>
    public class LogMessage : ILogMessage
    {
        /// <summary>
        /// The message to print to the console
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initialize a new instance of LogMessage
        /// </summary>
        /// <param name="message">The message to print to the console</param>
        public LogMessage(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Parse the log message into the full message to print in the log
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ">> " + Message;
        }
    }

    [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
    public static class ExtensionMethods
    {
        public static T AssertNotNull<T>(this T value) 
            where T : class
        {
            LogUtils.Assert(value != null, new TraceMessage($"Value is null. (Type: { typeof(T).Name })"));
            return value;
        }

        public static T AssertNotNull<T>(this T value, string message) where T : class
        {
            LogUtils.Assert(value != null, new TraceMessage(message));
            return value;
        }

        public static T AssertNotNull<T, TMessage>(this T value, TMessage message) 
            where T : class 
            where TMessage : struct, ILogMessage
        {
            LogUtils.Assert(value != null, message);
            return value;
        }

        public static T AssertNotNull<T>(this T? value) 
            where T : struct
        {
            LogUtils.Assert(value != null, new TraceMessage($"Value is null. (Type: { typeof(T).Name })"));
            return value.Value;
        }

        public static T AssertNotNull<T>(this T? value, string message) 
            where T : struct
        {
            LogUtils.Assert(value != null, new TraceMessage(message));
            return value.Value;
        }

        public static T AssertNotNull<T, TMessage>(this T? value, TMessage message) 
            where T : struct 
            where TMessage : struct, ILogMessage
        {
            LogUtils.Assert(value != null, message);
            return value.Value;
        }
    }
}