using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bunt.Core.Domain.Commands
{
    public class FlyttaNerBuntladeStalle
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly BuntDbContext _db;

            public Handler(BuntDbContext db)
            {
                _db = db;
            }

            public async Task Handle(Command command, CancellationToken cancellationToken)
            {
                var buntladeStalle = await _db.BuntladeStallen.SingleOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

                if (buntladeStalle == null || buntladeStalle.Index == 0)
                    return;

                var previousBuntladeStalle = await _db.BuntladeStallen.SingleOrDefaultAsync(b => b.Index == buntladeStalle.Index - 1, cancellationToken);

                if (previousBuntladeStalle == null)
                    return;

                buntladeStalle.Omsortera(buntladeStalle.Index - 1);
                previousBuntladeStalle.Omsortera(buntladeStalle.Index + 1);

                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}