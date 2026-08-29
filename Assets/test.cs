using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BizHawk.Client.Common;
using BizHawk.Common;
using BizHawk.Common.PathExtensions;
using BizHawk.Emulation.Common;
using UnityEngine;
using UnityHawk;

public class test : MonoBehaviour {
	public Rom m_Rom;

    // Start is called before the first frame update
    void Start()
    {
	    UnitySystemConsoleRedirector.Redirect();
	    // try cause init db throws in case db is already initialized
        try
        {
	        Database.InitializeDatabase(
		        bundledRoot: Path.Combine(PathUtils.ExeDirectoryPath, "gamedb"),
		        userRoot: Path.Combine(PathUtils.DataDirectoryPath, "gamedb"),
		        silent: true
	        );
        }
        catch
        {
	        // ignored
        }

        using var file = new HawkFile(Path.Combine(Application.dataPath, m_Rom.Location));
        var romGame = new RomGame(file);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

/// <summary>
/// Redirects writes to System.Console to Unity3D's Debug.Log.
/// </summary>
/// <author>
/// Jackson Dunstan, http://jacksondunstan.com/articles/2986
/// </author>
public static class UnitySystemConsoleRedirector
{
	private class UnityTextWriter : TextWriter
	{
		private StringBuilder buffer = new StringBuilder();

		public override void Flush()
		{
			Debug.Log(buffer.ToString());
			buffer.Length = 0;
		}

		public override void Write(string value)
		{
			buffer.Append(value);
			if (value != null)
			{
				var len = value.Length;
				if (len > 0)
				{
					var lastChar = value [len - 1];
					if (lastChar == '\n')
					{
						Flush();
					}
				}
			}
		}

		public override void Write(char value)
		{
			buffer.Append(value);
			if (value == '\n')
			{
				Flush();
			}
		}

		public override void Write(char[] value, int index, int count)
		{
			Write(new string (value, index, count));
		}

		public override Encoding Encoding
		{
			get { return Encoding.Default; }
		}
	}

	public static void Redirect()
	{
		Console.SetOut(new UnityTextWriter());
	}
}