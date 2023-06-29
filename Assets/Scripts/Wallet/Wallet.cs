using System;

public class Wallet
{
    private static float Gold
    {
        get => Data.Instance.Resources.Gold;
        set => Data.Instance.Resources.Gold.Value = value;
    }
    public static float GetGoldValue => Gold;

    public static bool SpendGold(in float amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        if (Gold < amount)
        {
            return false;
        }

        Gold -= amount;

        return true;
    }

    public static void ReceiveGold(in float amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        Gold += amount;
    }
}