using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Exceptions
{
    public class ValidationEXception : ApplicationException
    {
        public ValidationEXception()
            :base("one or more validation failure have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationEXception(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }
        public Dictionary<string, string[]> Errors { get; }
    }
}
