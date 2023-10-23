double sum = 0;
for (int x = 1; x <= 9; x++) {
    double x_d = (double)x;
    double temp = Math.Log2((x_d + 1) / x_d);
    Console.WriteLine(temp);
    sum += temp;
}
Console.WriteLine(sum);
Console.WriteLine(sum/9);
