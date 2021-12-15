using System;
using System.Linq;

//int[] input = { 3, 4, 3, 1, 2 };
int[] input = { 1,4,1,1,1,1,1,1,1,4,3,1,1,3,5,1,5,3,2,1,1,2,3,1,1,5,3,1,5,1,1,2,1,2,1,1,3,1,5,1,1,1,3,1,1,1,1,1,1,4,5,3,1,1,1,1,1,1,2,1,1,1,1,4,4,4,1,1,1,1,5,1,2,4,1,1,4,1,2,1,1,1,2,1,5,1,1,1,3,4,1,1,1,3,2,1,1,1,4,1,1,1,5,1,1,4,1,1,2,1,4,1,1,1,3,1,1,1,1,1,3,1,3,1,1,2,1,4,1,1,1,1,3,1,1,1,1,1,1,2,1,3,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,5,1,1,1,2,2,1,1,3,5,1,1,1,1,3,1,3,3,1,1,1,1,3,5,2,1,1,1,1,5,1,1,1,1,1,1,1,2,1,2,1,1,1,2,1,1,1,1,1,2,1,1,1,1,1,5,1,4,3,3,1,3,4,1,1,1,1,1,1,1,1,1,1,4,3,5,1,1,1,1,1,1,1,1,1,1,1,1,1,5,2,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,5,1,1,1,1,1,1,1,1,2,1,4,4,1,1,1,1,1,1,1,5,1,1,2,5,1,1,4,1,3,1,1 };

// Dictionary [int state, int count]
// state = number of days until spawn. 0..8 - 9 states in total
// count = number of fish in this state
// As States are numbers 0..8, this can be represented as an array

// Initialize world
long[] world = new long[9];
for (int state = 0; state < 9; state++)
{
    world[state] = input.Count(i => i == state); // count fish in this state
}

const int DAYS = 256;
for (int i = 0; i < DAYS; i++)
{
    world = SimulateNewDay(world);
}

long totalFish = world.Sum();
Console.WriteLine($"Total fish on day {DAYS}: {totalFish}");



long[] SimulateNewDay(long[] yesterdayWorld)
{
    long[] world = new long[9];
    // all fish become one day closer 
    for (int i = 0; i < 8; i++)
    {
        world[i] = yesterdayWorld[i + 1];
    }
    world[6] += yesterdayWorld[0]; // for adults, the timer resets
    world[8] = yesterdayWorld[0];  // this is the new spawn
    return world;
}
