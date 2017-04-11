using NUnit.Framework;
using FbxSdk;
using System.Collections.Generic;

namespace UnitTests
{
    public class FbxDocumentInfoTest : Base<FbxDocumentInfo>
    {
        private static Dictionary<string, string> m_dataValues = null;

        protected Dictionary<string, string> dataValues {
        	get {
        		if (m_dataValues == null) {
        			m_dataValues = new Dictionary<string, string> ()
        			{
						{ "title",      ".YvH5peIJMdg" },
						{ "subject",    "lmESAM8Fe3HV" },
						{ "author",     "hLsYMCqUekvr" },
						{ "revision",   "SknI2x=Ncp5P" },
						{ "keywords",   "netJRGcb8alS" },
						{ "comment",    ".0pzL-twb6mx" },
					};
        		}
        		return m_dataValues;
        	}
        }

        public static FbxDocumentInfo InitDocumentInfo (FbxDocumentInfo docInfo, Dictionary<string, string> values)
        {
            docInfo.mTitle = values ["title"];
            docInfo.mSubject = values ["subject"];
            docInfo.mAuthor = values ["author"];
            docInfo.mRevision = values ["revision"];
            docInfo.mKeywords = values ["keywords"];
            docInfo.mComment = values ["comment"];

            return docInfo;
        }

        public static void CheckDocumentInfo (FbxDocumentInfo docInfo, Dictionary<string, string> values)
        {
        	Assert.Equals (docInfo.mTitle, values ["title"]);
        	Assert.Equals (docInfo.mSubject, values ["subject"]);
        	Assert.Equals (docInfo.mAuthor, values ["author"]);
        	Assert.Equals (docInfo.mRevision, values ["revision"]);
        	Assert.Equals (docInfo.mKeywords, values ["keywords"]);
        	Assert.Equals (docInfo.mComment, values ["comment"]);
        }
        
        [Test]
        public void TestDocumentInfo ()
        {
            using (FbxDocumentInfo docInfo = FbxDocumentInfo.Create (FbxManager, "myDocument")) {

                CheckDocumentInfo (InitDocumentInfo (docInfo, this.dataValues), this.dataValues);
            }
        }
    }
}
