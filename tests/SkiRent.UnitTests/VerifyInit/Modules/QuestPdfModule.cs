using System.Runtime.CompilerServices;

using QuestPDF.Infrastructure;

namespace SkiRent.UnitTests.VerifyInit.Modules
{
    public class QuestPdfModule
    {
        protected QuestPdfModule()
        { }

        [ModuleInitializer]
        public static void Init()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            VerifyImageSharpCompare.Initialize();
            VerifyImageSharpCompare.RegisterComparers(threshold: 1);
            VerifyQuestPdf.Initialize();
        }
    }
}
