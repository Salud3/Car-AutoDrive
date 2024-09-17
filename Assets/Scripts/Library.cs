using System;

public static class Library 
{
    public static float Sigmoid(double v)
    {       
        return 1.0f / (1.0f + (float)Math.Exp(-v));
    }

    public static double HyperbolicTangtent(double x)
    {
        if (x < -45.0) return -1.0;
        else if (x > 45.0) return 1.0;
        else return Math.Tanh(x);
    }
}