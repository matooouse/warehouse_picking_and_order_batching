using System;
using System.Collections.Generic;

namespace warehouse_picking
{
    internal class ClientWish
    {
        public List<ClientWishPos> WishList { get; private set; }

        public ClientWish(int nbBlock, int nbAisles, int aisleLenght, int wishSize)
        {
            int nbProductMax = nbBlock*nbAisles*aisleLenght;
            int nbProductByBlock = nbAisles*aisleLenght;
            var wishList = new List<ClientWishPos>();
            var rnd = new Random();
            for (var i = 0; i < wishSize; i++)
            {
                int wishIdx = rnd.Next(1, nbProductMax + 1);
                int blockIdx = (wishIdx - 1)/nbProductByBlock + 1;
                int temp = wishIdx - (blockIdx - 1)*nbProductByBlock;
                int aislesIdx = (temp - 1)/aisleLenght + 1;
                temp = temp - (aislesIdx - 1)*aisleLenght;
                int positionIdx = temp;
                var wish = new ClientWishPos(wishIdx, blockIdx, aislesIdx, positionIdx, aisleLenght, nbBlock);
                wishList.Add(wish);
            }
            WishList = wishList;
        }
    }

    internal class ClientWishPos
    {
        public int WishIdx { get; private set; } //location in the article
        public int BlockIdx { get; private set; }
        public int AislesIdx { get; private set; }
        public int PositionIdx { get; private set; }
        public int WishX { get; private set; }
        public int WishY { get; private set; }
        public int UpperLeftX { get; private set; }
        public int UpperLeftY { get; private set; }
        public int BottomY { get; private set; }
        public int TopY { get; private set; }

        public ClientWishPos(int wishIdx, int blockIdx, int aislesIdx, int positionIdx, int aisleLenght, int nbBlock)
        {
            WishIdx = wishIdx;
            BlockIdx = blockIdx;
            AislesIdx = aislesIdx;
            PositionIdx = positionIdx;
            // une rangée est entourée de 2 couloirs
            WishY = (blockIdx - 1)*(aisleLenght + 2) + positionIdx;
            // aislesIdx = 1 => WishX = 1, aislesIdx = 2 => WishX = 2
            // aislesIdx = 3 => WishX = 4, aislesIdx = 4 => WishX = 5
            // aislesIdx = 5 => WishX = 7, aislesIdx = 6 => WishX = 8
            WishX = ((aislesIdx - 1)/2)*3 + aislesIdx%2;
            // on change le sens de Y pour pouvoir dessier depuis le coin supérieur gauche de l'écran
            UpperLeftY = (nbBlock - blockIdx)*(aisleLenght + 2) + (aisleLenght - positionIdx + 1);
            // aislesIdx = 1 => UpperLeftX = 0, aislesIdx = 2 => UpperLeftX = 2
            // aislesIdx = 3 => UpperLeftX = 3, aislesIdx = 4 => UpperLeftX = 5
            // aislesIdx = 5 => UpperLeftX = 6, aislesIdx = 6 => UpperLeftX = 8
            UpperLeftX = (aislesIdx/2)*3 + (aislesIdx%2 - 1);
            BottomY = (blockIdx - 1) * (aisleLenght + 2);
            TopY = BottomY + aisleLenght + 1;
        }
    }
}
