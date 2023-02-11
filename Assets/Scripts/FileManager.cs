using System.IO;

public static class FileManager
{
    public static string[] GetFilePathsInFolder(string path)
    {
		DirectoryInfo directoryInfo = new DirectoryInfo(path); // gets directory info from folder

		FileInfo[] files = directoryInfo.GetFiles(); // gets all files in folder

		string[] fileNames = new string[files.Length];

		// gets file names
		for (int i = 0; i < files.Length; i++)
		{
			fileNames[i] = files[i].FullName;
		}

		return fileNames;
	}

	public static FileInfo[] GetFileInfoInFolder(string path)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(path); // gets directory info from folder

		FileInfo[] files = directoryInfo.GetFiles(); // gets all files in folder

		return files;
	}
}
