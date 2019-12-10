using System;
using System.Collections.Generic;
using System.Linq;

namespace warehouse_picking_core
{
    internal class Pickings : IPickings
    {
        public List<PickingPos> PickingList { get; private set; }

        internal Pickings(Warehouse w, int wishSize)
        {
            int nbBlock = w.NbBlock;
            int nbAisles = w.NbAisles;
            int aisleLenght = w.AisleLenght;
            int nbProductMax = nbBlock*nbAisles*aisleLenght;
            int nbProductByBlock = nbAisles*aisleLenght;
            var wishList = new HashSet<PickingPos>();
            var rnd = new Random();
            for (var i = 0; i < wishSize; i++)
            {
                int wishIdx = rnd.Next(1, nbProductMax + 1);
                int blockIdx = (wishIdx - 1)/nbProductByBlock + 1;
                int temp = wishIdx - (blockIdx - 1)*nbProductByBlock;
                int aislesIdx = (temp - 1)/aisleLenght + 1;
                temp = temp - (aislesIdx - 1)*aisleLenght;
                int positionIdx = temp;
                var wish = new PickingPos(wishIdx, blockIdx, aislesIdx, positionIdx, aisleLenght, nbBlock);
                wishList.Add(wish);
            }
            PickingList = wishList.OrderBy(x => x.WishIdx).ToList();
        }
    }

    public interface IPickings
    {
        List<PickingPos> PickingList { get; }
    }

    public class PickingPos
    {
        public int WishIdx { get; private set; } //location in the article
        public int BlockIdx { get; private set; }
        public int AislesIdx { get; private set; }
        public int PositionIdx { get; private set; }
        public int PickingPointX { get; private set; }
        public int PickingPointY { get; private set; }
        public int UpperLeftX { get; private set; }
        public int UpperLeftY { get; private set; }
        public int BottomY { get; private set; }
        public int TopY { get; private set; }

        internal PickingPos(int wishIdx, int blockIdx, int aislesIdx, int positionIdx, int aisleLenght, int nbBlock)
        {
            WishIdx = wishIdx;
            BlockIdx = blockIdx;
            AislesIdx = aislesIdx;
            PositionIdx = positionIdx;
            // une rangée est entourée de 2 couloirs
            PickingPointY = (blockIdx - 1) * (aisleLenght + 2) + positionIdx;
            // aislesIdx = 1 => PickingPointX = 1, aislesIdx = 2 => PickingPointX = 1
            // aislesIdx = 3 => PickingPointX = 4, aislesIdx = 4 => PickingPointX = 4
            // aislesIdx = 5 => PickingPointX = 7, aislesIdx = 6 => PickingPointX = 7
            PickingPointX = ((aislesIdx - 1) / 2) * 3 + 1;
            // on change le sens de Y pour pouvoir dessier depuis le coin supérieur gauche de l'écran
            UpperLeftY = (nbBlock - blockIdx)*(aisleLenght + 2) + (aisleLenght - positionIdx + 1);
            // aislesIdx = 1 => UpperLeftX = 0, aislesIdx = 2 => UpperLeftX = 2
            // aislesIdx = 3 => UpperLeftX = 3, aislesIdx = 4 => UpperLeftX = 5
            // aislesIdx = 5 => UpperLeftX = 6, aislesIdx = 6 => UpperLeftX = 8
            UpperLeftX = (aislesIdx/2)*3 + (aislesIdx%2 - 1);
            BottomY = (blockIdx - 1)*(aisleLenght + 2);
            TopY = BottomY + aisleLenght + 1;
        }

        public override string ToString()
        {
            return "WishIdx : " + WishIdx
                   + ", BlockIdx : " + BlockIdx
                   + ", AislesIdx : " + AislesIdx
                   + ", PositionIdx : " + PositionIdx;
        }

        public override int GetHashCode()
        {
            return WishIdx;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            var other = (PickingPos) obj;
            return WishIdx == other.WishIdx;
        }
    }

    internal static class ClientWishPosExtention
    {
        internal static ShiftPoint ConverToShiftPoint(this PickingPos c)
        {
            return new ShiftPoint(c.PickingPointX, c.PickingPointY);
        }
    }  
}
