using System.Threading.Tasks;

namespace generic_kafka_library.Interface;

public interface IKafkaHandler<Tk, Tv>
{
    Task HandleAsync(Tk key, Tv value);
}