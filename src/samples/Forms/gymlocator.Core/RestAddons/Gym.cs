using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gymlocator.Rest.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class Gym
    {
        public string Email
        {
            get
            {
                return Contact.Email;
            }
        }

        public int NoPT
        {
            get
            {
                return PersonalTrainers.Count();
            }
        }

        public bool HasPT {
            get {
                return NoPT > 0;
            }
        }
    }

    public static class GymExtensions
    {
        public static async Task<List<Gym>> GetGymListAsync(this IGymAPI operations, string locale, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await operations.GetGymsWithHttpMessagesAsync(locale, null, cancellationToken).ConfigureAwait(false))
            {
                return (_result.Body as IEnumerable<Gym>).OrderBy(d => d.Name).ToList();
            }
        }
    }
}
