using System;

namespace FbxSdk.Examples
{
	public class ExportSettings
	{
		/// <summary>
		/// Gets the export method.
		/// </summary>
		/// <value>The export method.</value>
		public ExportMethod Method { get; private set; }

		/// <summary>
		/// Gets the destination path.
		/// </summary>
		/// <value>The destination path.</value>
		public string DestinationPath { get; private set; }

		/// <summary>
		/// Gets the scale we are exporting at.
		/// </summary>
		/// <value>The scale.</value>
		public float Scale { get; private set; }

		///<summary>
		/// Which way should the axes be oriented.
		/// </summary>
		public enum AxesSettings
		{
			LeftHandedYUp,
			RightHandedYUp,
			RightHandedZUp
		}

		/// <summary>
		/// Gets the axes.
		/// </summary>
		/// <value>The axes.</value>
		public AxesSettings Axes { get; private set; }

		public ExportSettings (ExportMethod method, string path, float scale, AxesSettings axes)
		{
			Method = method;
			DestinationPath = path;
			Scale = scale;
			Axes = axes;
		}

		/// <summary>
		/// Copies the ExportSettings, but changes the path to the new path.
		/// </summary>
		/// <returns>A copy of the ExportSettings with the new path.</returns>
		/// <param name="newPath">New path.</param>
		public ExportSettings CopyWithChangedPath (string newPath)
		{
			return new ExportSettings (Method, newPath, Scale, Axes);
		}

		/// <summary>
		/// Copies the ExportSettings, but changes the path to the new scale.
		/// </summary>
		/// <returns>A copy of the ExportSettings with the new scale.</returns>
		/// <param name="newScale">New scale.</param>
		public ExportSettings CopyWithChangedScale (float newScale)
		{
			return new ExportSettings (Method, DestinationPath, newScale, Axes);
		}
	}
}