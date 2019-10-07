using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcModel {
    public Npc.Type NpcType { get; set; }
    public ItemData.Type Item { get; set; } // Can either be the wanted item or the item that the npc is holding (depending on the NpcType)

    public int InitialOfferAmount { get; set; }
    public int MaxOfferAmount { get; set; }
    public int AmountThresholdForLeaving { get; set; }
    public int AmountOfOffers { get; set; }

    public int MaxOfferIncrement {
        get {
            if (NpcType == Npc.Type.Buying) {
                var offerDelta = MaxOfferAmount - InitialOfferAmount;
                return offerDelta / AmountOfOffers;
            }

            if (NpcType == Npc.Type.Selling) {
                var offerDelta = InitialOfferAmount - MaxOfferAmount;
                return offerDelta / AmountOfOffers;
            }

            Debug.LogWarning("[NpcModel] Cannot compute max offer increment for npc type: " + NpcType.ToString());
            return 0;
        }
    }
}
