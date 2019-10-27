using NFluent;
using NUnit.Framework;

namespace warehouse_picking.Tests
{
    [TestFixture]
    class ClientWishPosTests
    {

        [Test]
        public void Should_test_the_formula_for_wish_coordonates()
        {
            var nbBlock = 1;
            var blockIdx = 1;
            var aislesIdx = 1;
            var position = 1;
            var aislesLenght = 1;
            var wish = new ClientWishPos(1, blockIdx, aislesIdx, position, aislesLenght, nbBlock);
            Check.That(wish.WishX).Equals(1);
            //X do not change for block
            nbBlock = 2;
            blockIdx = 2;
            int wantedX = 1;
            wish = new ClientWishPos(1, blockIdx, aislesIdx, position, aislesLenght, nbBlock);
            Check.That(wish.WishX).Equals(wantedX);
            Check.That(wish.UpperLeftX).Equals(0);
            //X do not change for aislesLenght
            aislesLenght = 2;
            wish = new ClientWishPos(1, blockIdx, aislesIdx, position, aislesLenght, nbBlock);
            Check.That(wish.WishX).Equals(wantedX);
            Check.That(wish.UpperLeftX).Equals(0);
            // X do change for aislesidx
            aislesIdx = 2;
            wantedX = 2;
            wish = new ClientWishPos(1, blockIdx, aislesIdx, position, aislesLenght, nbBlock);
            Check.That(wish.WishX).Equals(wantedX);
            Check.That(wish.UpperLeftX).Equals(2);
            // X do change for aislesidx
            aislesIdx = 3;
            wantedX = 4;
            wish = new ClientWishPos(1, blockIdx, aislesIdx, position, aislesLenght, nbBlock);
            Check.That(wish.WishX).Equals(wantedX);
            Check.That(wish.UpperLeftX).Equals(3);
        }
    }
}
