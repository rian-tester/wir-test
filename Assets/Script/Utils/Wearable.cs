using System;

[Serializable]
public class Wearable
{
    [Serializable]
    public class FileMeta
    {
        public string configUrl;
        public string manifestUrl;
        public string assetBundleUrl;
    }
    public string id,
        wearableId,
        wearableName,
        tokenId,
        trxHash,
        amount,
        version,
        createdAt,
        updatedAt;
    public FileMeta fileMeta;
}
