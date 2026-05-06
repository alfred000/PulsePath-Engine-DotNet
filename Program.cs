using PulsePath_Engine_DotNet.Business;

double myBmr = MetabolicEngine.CalculateBMR(80, 180, 30, true);
double myFactor = MetabolicEngine.GetActivityFactor(12000);
double myTdee = MetabolicEngine.CalculateTDEE(myBmr, myFactor);

Console.WriteLine($"Mon TDEE aujourd'hui : {myTdee} kcal");
