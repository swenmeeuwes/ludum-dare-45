using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcModel {
    public Npc.Type NpcType { get; set; }
    public ItemData.Type WantedItem { get; set; }

    public int InitialOfferAmount { get; set; }
    public int MaxOfferAmount { get; set; }
    public int AmountThresholdForLeaving { get; set; }
    public int AmountOfOffers { get; set; }

    public int OfferIncrement {
        get {
            var offerDelta = MaxOfferAmount - InitialOfferAmount;
            return offerDelta / AmountOfOffers;
        }
    }
}
