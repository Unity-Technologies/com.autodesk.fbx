namespace FbxSdk.Examples
{
    public class ExportMethod
    {
        /// <summary>
        /// Gets a user-visible name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the file extension typically associated with this method.
        /// </summary>
        public string Extension { get; private set; }

        /// <summary>
        /// Gets the MIME format for this method, if any.
        /// </summary>
        public string MimeFormat { get; private set; }

        /// <summary>
        /// Gets the typical axes layout for this file format.
        /// </summary>
        public ExportSettings.AxesSettings DefaultAxes { get; private set; }

        /// <summary>
        /// The export type. This is known at construction time to be a sublcass of AbstractExporter.
        /// </summary>
        public System.Type ExportType { get; private set; }

        /// <summary>
        /// Private constructor: you have to use ConstructMethod to call it.
        /// </summary>
        ExportMethod (string name, string extension, string mime, ExportSettings.AxesSettings axes, System.Type type)
        {
            Name = name;
            Extension = extension;
            MimeFormat = mime;
            DefaultAxes = axes;
            ExportType = type;
        }

        /// <summary>
        /// Create a method.
        /// </summary>
        public static ExportMethod ConstructMethod<Exporter> (string name, string extension, string mime, ExportSettings.AxesSettings axes) where Exporter : AbstractExporter
        {
            return new ExportMethod (name, extension, mime, axes, typeof (Exporter));
        }

        /// <summary>
        /// Return the default settings for this method, for output towards the given path and scale.
        /// </summary>
        public ExportSettings DefaultSettings (string path, float scale)
        {
            return new ExportSettings (this, path, scale, DefaultAxes);
        }

        /// <summary>
        /// Instantiate the exporter with the given settings.
        /// </summary>
        public AbstractExporter Instantiate (ExportSettings settings)
        {
            return (AbstractExporter)System.Activator.CreateInstance (ExportType, settings);
        }

        /// <summary>
        /// Instantiate the exporter with the default settings.
        /// </summary>
        public AbstractExporter Instantiate (string path, float scale)
        {
            return Instantiate (DefaultSettings (path, scale));
        }

        /// <summary>
        /// Return the given path or filename with the extension that matches this file format.
        /// </summary>
        public string ExtendPath (string filename)
        {
            if (string.IsNullOrEmpty (Extension)) {
                return filename;
            }

            var extension = System.IO.Path.GetExtension (filename).TrimStart ('.').ToLower ();
            if (extension == Extension) {
                return filename;
            } else {
                return filename + '.' + Extension;
            }
        }

    }
}