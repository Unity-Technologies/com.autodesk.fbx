using NUnit.Framework;
using FbxSdk;
using System.Collections.Generic;

namespace UnitTests
{
    public class FbxDocumentTest : Base<FbxDocument>
    {
        private static Dictionary<string, string> m_dataValues = null;

        protected Dictionary<string, string> dataValues {
        	get {
        		if (m_dataValues == null) {
        			m_dataValues = new Dictionary<string, string> ()
        			{
						{ "title",      "Document Title" },
						{ "subject",    "Unit Tests for DocumentInfo class." },
						{ "author",     "Unity Technolies" },
						{ "revision",   "1.0" },
						{ "keywords",   "do not crash" },
						{ "comment",    "Testing the DocumentInfo object." },
					};
        		}
        		return m_dataValues;
        	}
        }

        [Test]
        public void TestDocumentInfo ()
        {
            using (FbxDocument doc = FbxDocument.Create (FbxManager, "myDocument"))
            {
                using (FbxDocumentInfo docInfo = FbxDocumentInfo.Create (FbxManager, "myDocument")) {
                    doc.SetDocumentInfo (FbxDocumentInfoTest.InitDocumentInfo(docInfo, this.dataValues));

                    FbxDocumentInfoTest.CheckDocumentInfo (doc.GetDocumentInfo(), this.dataValues);

                    //  Assert.ReferenceEquals (docInfo, doc.GetDocumentInfo ());
                }
            }
        }
    }
}
