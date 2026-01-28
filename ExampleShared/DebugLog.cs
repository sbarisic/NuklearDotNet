using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace ExampleShared
{
	/// <summary>
	/// Simple debug logging utility that writes to out.txt for crash investigation.
	/// </summary>
	public static class DebugLog
	{
		private static readonly object _lock = new object();
		private static readonly string LogFile = "out.txt";
		private static bool _initialized = false;

		/// <summary>
		/// Initialize the log file (clears previous content).
		/// </summary>
		public static void Init()
		{
			lock (_lock)
			{
				try
				{
					File.WriteAllText(LogFile, $"=== NuklearDotNet Debug Log - {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===\n\n");
					_initialized = true;
					Log("Debug logging initialized");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to initialize debug log: {ex.Message}");
				}
			}
		}

		/// <summary>
		/// Log a message with timestamp and optional caller info.
		/// </summary>
		public static void Log(string message,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string filePath = "",
			[CallerLineNumber] int lineNumber = 0)
		{
			lock (_lock)
			{
				try
				{
					if (!_initialized) Init();

					string fileName = Path.GetFileName(filePath);
					string logEntry = $"[{DateTime.Now:HH:mm:ss.fff}] [{fileName}:{lineNumber}] {memberName}: {message}\n";

					File.AppendAllText(LogFile, logEntry);

					// Also write to console for immediate visibility
					Console.Write(logEntry);
				}
				catch
				{
					// Silently fail to avoid disrupting the app
				}
			}
		}

		/// <summary>
		/// Log an error with exception details.
		/// </summary>
		public static void Error(string message, Exception ex = null,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string filePath = "",
			[CallerLineNumber] int lineNumber = 0)
		{
			string errorMsg = ex != null
				? $"ERROR: {message} - {ex.GetType().Name}: {ex.Message}\nStack: {ex.StackTrace}"
				: $"ERROR: {message}";
			Log(errorMsg, memberName, filePath, lineNumber);
		}

		/// <summary>
		/// Log entry into a method/section.
		/// </summary>
		public static void Enter([CallerMemberName] string memberName = "")
		{
			Log($">>> Entering", memberName);
		}

		/// <summary>
		/// Log exit from a method/section.
		/// </summary>
		public static void Exit([CallerMemberName] string memberName = "")
		{
			Log($"<<< Exiting", memberName);
		}

		/// <summary>
		/// Flush the log file to ensure all content is written.
		/// </summary>
		public static void Flush()
		{
			// File.AppendAllText already flushes, but this is here for API completeness
			Log("Log flushed");
		}
	}
}
