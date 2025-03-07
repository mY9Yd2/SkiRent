namespace SkiRent.Desktop.Contracts
{
    public interface IInitializeAsync<in T1, in T2, in T3, in T4>
    {
        public Task InitializeAsync(T1 data1, T2 data2, T3 data3, T4 data4);
    }

    public interface IInitializeAsync<in T1, in T2, in T3>
    {
        public Task InitializeAsync(T1 data1, T2 data2, T3 data3);
    }

    public interface IInitializeAsync<in T1, in T2>
    {
        public Task InitializeAsync(T1 data1, T2 data2);
    }

    public interface IInitializeAsync<in T>
    {
        public Task InitializeAsync(T data);
    }

    public interface IInitializeAsync
    {
        public Task InitializeAsync();
    }
}
