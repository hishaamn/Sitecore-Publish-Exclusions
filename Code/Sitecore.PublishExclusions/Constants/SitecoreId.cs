namespace Sitecore.PublishExclusions.Constants
{
    using Sitecore.Data;

    public static class SitecoreId
    {
        public static class TemplateId
        {
            public static readonly ID PublishConfiguration = new ID("{86B50187-E9F1-43D0-96A6-6DF35D6F0614}");

            public static readonly ID PublishExclusion = new ID("{2FC512C4-F934-410F-A5B4-A5F0176B6DB0}");
        }

        public static class FieldId
        {
            public static readonly ID PublishingTarget = new ID("{80C3E73A-ABFB-4324-98FF-553BDF276BDF}");

            public static readonly ID PublishingMode = new ID("{6F787F22-07D5-4D31-AA62-344E13E2F80F}");

            public static readonly ID ExcludedNode = new ID("{BA065FCD-3F5A-4284-8FC1-C975EDAF4D62}");

            public static readonly ID ExcludedNodeOverride = new ID("{59FF79F6-BFEF-47A2-80E7-A835D359FC3D}");

            public static readonly ID ReturnToQueue = new ID("{7CCB6040-8911-4D1F-A155-FBC6DF5FB2ED}");

            public static readonly ID ContentEditorWarning = new ID("{2D2AF637-627B-45E8-AE40-58D516B7F416}");
        }

        public static class ItemId
        {
            public static readonly ID ExclusionContainer = new ID("{617B920B-4DE7-4346-806C-C8189598B667}");

            public static readonly ID ExclusionConfiguration = new ID("{DA0F5A50-1B2C-49EC-84DD-FB2C3F0AD838}");
        }
    }
}
