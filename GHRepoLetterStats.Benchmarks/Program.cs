// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using GHRepoLetterStats.Benchmarks;

var summary = BenchmarkRunner.Run<Benchmark>();
