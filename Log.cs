using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;

namespace SonicRemake
{
  public class Log
  {
    private const string Gray = "\u001B[90m";
    private const string Red = "\u001B[31m";
    private const string Green = "\u001B[32m";
    private const string Yellow = "\u001B[33m";
    private const string Blue = "\u001B[34m";
    private const string Magenta = "\u001B[35m";
    private const string Cyan = "\u001B[36m";
    private const string White = "\u001B[37m";

    private const int StdOutputHandle = -11;
    private const uint EnableVirtualTerminalProcessing = 0x0004;
    private readonly Type _callerType;

    public Log(Type callerType)
    {
      _callerType = callerType;
    }

    public static Log Send()
    {
      var frame = new StackFrame(1);
      var method = frame.GetMethod();
      var type = method.DeclaringType!;

      return new Log(type);
    }

    private void LogMessage(LogLevel level, Exception? exception, string message)
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        // Get the STD handle
        var iStdOut = GetStdHandle(StdOutputHandle);

        // Try to enable the use of ANSI codes
        var colorSupported = GetConsoleMode(iStdOut, out var outConsoleMode) &&
                             SetConsoleMode(iStdOut, outConsoleMode | EnableVirtualTerminalProcessing);
      }

      var builder = new StringBuilder();

      // [FATAL]
      builder.Append(Gray);
      builder.Append(' ');
      builder.Append(GetColorCode(level));
      builder.Append(GetLevel(level));
      builder.Append(Gray);
      builder.Append(" |");

      // [Class.Method:line]
      builder.Append(Gray);
      builder.Append(' ');
      builder.Append(BuildStackTraceElement());
      builder.Append(Gray);
      builder.Append(": ");

      // Message
      builder.Append(GetColorCode(level));
      builder.Append(message);

      if (exception != null)
      {
        var ex = exception;

        while (ex != null)
        {
          builder.AppendLine();
          builder.Append("         ");
          builder.Append(Red);
          builder.Append(exception.GetType().Name.Replace("Exception", string.Empty));
          builder.Append(Gray);
          builder.Append("! ");
          builder.Append(GetColorCode(level));
          builder.Append(exception.Message);

          if (exception.Data.Count > 0)
          {
            builder.Append(Gray);
            builder.AppendLine();
            builder.Append("           ");
            builder.Append(JsonConvert.SerializeObject(exception.Data));
          }

          ex = ex.InnerException;
        }

        if (exception.StackTrace != null)
        {
          var stackTraceElements = exception.StackTrace.Split('\n').Select(x => x.Trim()).ToList();

          foreach (var stackTraceElement in stackTraceElements)
          {
            builder.AppendLine();
            builder.Append(Gray);
            builder.Append("           ");
            builder.Append(stackTraceElement.Split(" in ").First());
          }
        }
      }

      builder.Append(White);

      Console.WriteLine(builder.ToString());
    }

    private string GetLevel(LogLevel level)
    {
      switch (level)
      {
        case LogLevel.Debug:
          return "DEBUG";
        case LogLevel.Information:
          return " INFO";
        case LogLevel.Warning:
          return " WARN";
        case LogLevel.Error:
          return "ERROR";
        case LogLevel.Critical:
          return "FATAL";
        default:
          throw new ArgumentOutOfRangeException(nameof(level), level, null);
      }
    }

    public void Debug(Exception exception, string message)
    {
      LogMessage(LogLevel.Debug, exception, message);
    }

    public void Debug(string message)
    {
      LogMessage(LogLevel.Debug, null, message);
    }

    public void Information(Exception exception, string message)
    {
      LogMessage(LogLevel.Information, exception, message);
    }

    public void Information(string message)
    {
      LogMessage(LogLevel.Information, null, message);
    }

    public void Warning(Exception exception, string message)
    {
      LogMessage(LogLevel.Warning, exception, message);
    }

    public void Warning(string message)
    {
      LogMessage(LogLevel.Warning, null, message);
    }

    public void Error(Exception exception, string message)
    {
      LogMessage(LogLevel.Error, exception, message);
    }

    public void Error(string message)
    {
      LogMessage(LogLevel.Error, null, message);
    }

    public void Critical(Exception exception, string message)
    {
      LogMessage(LogLevel.Critical, exception, message);
    }

    public void Critical(string message)
    {
      LogMessage(LogLevel.Critical, null, message);
    }

    private string GetColorCode(LogLevel level)
    {
      return level switch
      {
        LogLevel.Debug => Gray,
        LogLevel.Information => Blue,
        LogLevel.Warning => Yellow,
        LogLevel.Error => Red,
        LogLevel.Critical => Magenta,
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
      };
    }

    private string BuildStackTraceElement()
    {
      var builder = new StringBuilder();

      builder.Append(Green);
      builder.Append(_callerType.Name);

      return builder.ToString();
    }

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);
  }

  public enum LogLevel
  {
    Debug,
    Information,
    Warning,
    Error,
    Critical
  }
}
