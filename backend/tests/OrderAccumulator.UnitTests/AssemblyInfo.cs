using Xunit;

// Os testes que usam o mesmo PostgreSQL são executados sequencialmente para evitar interferência.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
