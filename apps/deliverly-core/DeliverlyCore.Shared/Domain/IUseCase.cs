namespace DeliverlyCore.Shared.Domain
{
    public interface IUseCase<in TInput, TOutput>
    {
        Task<Result<TOutput>> ExecuteAsync(TInput input, CancellationToken ct = default);
    }
}
