using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace ValorantReplayParser.Models;

[NetFieldExportGroup("/Script/ShooterGame.PurchasedItemComponent", minimalParseMode: ParseMode.Normal)]
public class PurchasedItemComponent : INetFieldExportGroup
{
    [NetFieldExport("Purchaseable", RepLayoutCmdType.Ignore)]
    public uint? Purchaseable { get; set; }

    [NetFieldExport("bIsCurrentSessionPurchase", RepLayoutCmdType.PropertyBool)]
    public bool? bIsCurrentSessionPurchase { get; set; }

    [NetFieldExport("PurchasingPlayerState", RepLayoutCmdType.PropertyObject)]
    public uint? PurchasingPlayerState { get; set; }

    [NetFieldExport("PurchasableTransactionSource", RepLayoutCmdType.Ignore)]
    public uint? PurchasableTransactionSource { get; set; }
}
