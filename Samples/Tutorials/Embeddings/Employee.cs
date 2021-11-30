// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Embeddings;
using Dolittle.SDK.Projections;

[Embedding("e5577d2c-0de7-481c-b5be-6ef613c2fcd6")]
public class Employee
{
    public string Name { get; set; } = "";
    public string Workplace { get; set; } = "Unassigned";

    public object ResolveUpdateToEvents(Employee updatedEmployee, EmbeddingContext context)
    {
        if (Name != updatedEmployee.Name)
        {
            return new EmployeeHired(updatedEmployee.Name);
        }
        else if (Workplace != updatedEmployee.Workplace)
        {
            return new EmployeeTransferred(Name, Workplace, updatedEmployee.Workplace);
        }

        throw new NotImplementedException();
    }

    public object ResolveDeletionToEvents(EmbeddingContext context)
    {
        return new EmployeeRetired(Name);
    }

    public void On(EmployeeHired @event, EmbeddingProjectContext context)
    {
        Name = @event.Name;
    }

    public void On(EmployeeTransferred @event, EmbeddingProjectContext context)
    {
        Workplace = @event.To;
    }

    public ProjectionResultType On(EmployeeRetired @event, EmbeddingProjectContext context)
    {
        return ProjectionResultType.Delete;
    }
}
