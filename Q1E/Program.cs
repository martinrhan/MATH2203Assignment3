int Simulate() {
    Random random = new Random();
    int Y = 0;
    int X = 0;
    while (Y == 0) {
        X = random.Next(1, 10);
        double X_d = (double)X;
        double f_D = Math.Log2((X_d + 1) / X_d);
        Y = random.NextDouble() < f_D ? 1 : 0;
    }
    return X;
}

for (int i = 0; i < 10; i++) {
    Console.WriteLine(Simulate());
}
