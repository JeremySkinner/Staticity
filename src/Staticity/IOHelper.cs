namespace Staticity {
	using System.IO;

	public class IOHelper {
		public static void DeleteDirectory(string path) {
			var dirInfo = new DirectoryInfo(path);
			SetAttributesNormal(dirInfo);
			dirInfo.Delete(true);
		}

		static void DeleteFile(string path) {
			var file = new FileInfo(path);

			if (!file.Exists) return;

			//Ensure that the file attributes are set to Normal so we can do the delete
			if (file.Attributes != FileAttributes.Normal) {
				file.Attributes = FileAttributes.Normal;
			}

			File.Delete(path);
		}

		/// <summary>
		///   The Delete() method will fail with UnauthorizedAccessException if any files in the directory tree have the read-only flag. 
		///   Delete() cannot delete anything with read-only flag even if the user running the application has priviliges to delete these files.
		/// </summary>
		/// <param name="dir"></param>
		static void SetAttributesNormal(DirectoryInfo dir) {
			// Remove flags from the current directory
			dir.Attributes = FileAttributes.Normal;

			// Remove flags from all files in the current directory
			foreach (FileInfo file in dir.GetFiles()) {
				file.Attributes = FileAttributes.Normal;
			}

			// Do the same for all subdirectories
			foreach (DirectoryInfo subDir in dir.GetDirectories()) {
				SetAttributesNormal(subDir);
			}
		}

		public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target) {
			if(! target.Exists) {
				target.Create();
			}
			foreach (DirectoryInfo dir in source.GetDirectories())
				CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
			foreach (FileInfo file in source.GetFiles())
				file.CopyTo(Path.Combine(target.FullName, file.Name));
		}
 
	}
}