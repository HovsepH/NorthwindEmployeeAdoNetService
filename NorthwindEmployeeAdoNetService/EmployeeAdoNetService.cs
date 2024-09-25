using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Net;

namespace NorthwindEmployeeAdoNetService;

/// <summary>
/// A service for interacting with the "Employees" table using ADO.NET.
/// </summary>
public sealed class EmployeeAdoNetService
{
    private DbProviderFactory _dbFactory;
    private string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeAdoNetService"/> class.
    /// </summary>
    /// <param name="dbFactory">The database provider factory used to create database connection and command instances.</param>
    /// <param name="connectionString">The connection string used to establish a database connection.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="dbFactory"/> or <paramref name="connectionString"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="connectionString"/> is empty or contains only white-space characters.</exception>
    public EmployeeAdoNetService(DbProviderFactory dbFactory, string connectionString)
    {
        _connectionString = string.IsNullOrEmpty(connectionString) ? throw new ArgumentNullException(nameof(connectionString)) : connectionString;
        _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
    }

    /// <summary>
    /// Retrieves a list of all employees from the Employees table of the database.
    /// </summary>
    /// <returns>A list of Employee objects representing the retrieved employees.</returns>
    public IList<Employee> GetEmployees()
    {
        using (DbConnection? connection = _dbFactory.CreateConnection())
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            connection.ConnectionString = _connectionString;
            connection.Open();

            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Employees";
                List<Employee> employees = new();

                using (var reader = command?.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee((long)reader.GetInt32(reader.GetOrdinal("EmployeeID")))
                            {
#pragma warning disable CS8601
                                FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
#pragma warning restore CS8601
                                Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                                TitleOfCourtesy = reader.IsDBNull(reader.GetOrdinal("TitleOfCourtesy")) ? null : reader.GetString(reader.GetOrdinal("TitleOfCourtesy")),
                                BirthDate = reader.IsDBNull(reader.GetOrdinal("BirthDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                                HireDate = reader.IsDBNull(reader.GetOrdinal("HireDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("HireDate")),
                                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                                City = reader.IsDBNull(reader.GetOrdinal("City")) ? null : reader.GetString(reader.GetOrdinal("City")),
                                Region = reader.IsDBNull(reader.GetOrdinal("Region")) ? null : reader.GetString(reader.GetOrdinal("Region")),
                                PostalCode = reader.IsDBNull(reader.GetOrdinal("PostalCode")) ? null : reader.GetString(reader.GetOrdinal("PostalCode")),
                                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader.GetString(reader.GetOrdinal("Country")),
                                HomePhone = reader.IsDBNull(reader.GetOrdinal("HomePhone")) ? null : reader.GetString(reader.GetOrdinal("HomePhone")),
                                Extension = reader.IsDBNull(reader.GetOrdinal("Extension")) ? null : reader.GetString(reader.GetOrdinal("Extension")),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                ReportsTo = reader.IsDBNull(reader.GetOrdinal("ReportsTo")) ? (long?)null : (long)reader.GetInt32(reader.GetOrdinal("ReportsTo")),
                                PhotoPath = reader.IsDBNull(reader.GetOrdinal("PhotoPath")) ? null : reader.GetString(reader.GetOrdinal("PhotoPath")),
                            });
                        }
                    }
                }
                return employees;
            }
        }
    }

    /// <summary>
    /// Retrieves an employee with the specified employee ID.
    /// </summary>
    /// <param name="employeeId">The ID of the employee to retrieve.</param>
    /// <returns>The retrieved an <see cref="Employee"/> instance.</returns>
    /// <exception cref="EmployeeServiceException">Thrown if the employee is not found.</exception>
    public Employee GetEmployee(long employeeId)
    {
        using var connection = _dbFactory.CreateConnection() ?? throw new ArgumentException("connection is null");
        connection.ConnectionString = _connectionString;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            @"
                SELECT * FROM Employees e WHERE e.EmployeeID == @employeeId;
            ";

        var idParameter = command.CreateParameter();
        idParameter.ParameterName = "@employeeId";
        idParameter.Value = employeeId;
        command.Parameters.Add(idParameter);

        Employee? employee = null;
        using var reader = command.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                employee = new Employee(employeeId)
                {
#pragma warning disable CS8601
                    FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
#pragma warning restore CS8601
                    Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                    TitleOfCourtesy = reader.IsDBNull(reader.GetOrdinal("TitleOfCourtesy")) ? null : reader.GetString(reader.GetOrdinal("TitleOfCourtesy")),
                    BirthDate = reader.IsDBNull(reader.GetOrdinal("BirthDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                    HireDate = reader.IsDBNull(reader.GetOrdinal("HireDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("HireDate")),
                    Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                    City = reader.IsDBNull(reader.GetOrdinal("City")) ? null : reader.GetString(reader.GetOrdinal("City")),
                    Region = reader.IsDBNull(reader.GetOrdinal("Region")) ? null : reader.GetString(reader.GetOrdinal("Region")),
                    PostalCode = reader.IsDBNull(reader.GetOrdinal("PostalCode")) ? null : reader.GetString(reader.GetOrdinal("PostalCode")),
                    Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader.GetString(reader.GetOrdinal("Country")),
                    HomePhone = reader.IsDBNull(reader.GetOrdinal("HomePhone")) ? null : reader.GetString(reader.GetOrdinal("HomePhone")),
                    Extension = reader.IsDBNull(reader.GetOrdinal("Extension")) ? null : reader.GetString(reader.GetOrdinal("Extension")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                    ReportsTo = reader.IsDBNull(reader.GetOrdinal("ReportsTo")) ? (long?)null : (long)reader.GetInt32(reader.GetOrdinal("ReportsTo")),
                    PhotoPath = reader.IsDBNull(reader.GetOrdinal("PhotoPath")) ? null : reader.GetString(reader.GetOrdinal("PhotoPath")),
                };
            }
        }

        if (employee is null)
        {
            throw new EmployeeServiceException("Employee not found.");
        }
        return employee;
    }

    /// <summary>
    /// Adds a new employee to Employee table of the database.
    /// </summary>
    /// <param name="employee">The  <see cref="Employee"/> object containing the employee's information.</param>
    /// <returns>The ID of the newly added employee.</returns>
    /// <exception cref="EmployeeServiceException">Thrown when an error occurs while adding the employee.</exception>
    public long AddEmployee(Employee employee)
    {
        using var connection = _dbFactory.CreateConnection() ?? throw new ArgumentException("connection is null");
        connection.ConnectionString = _connectionString;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
        @"
            INSERT INTO Employees 
            (FirstName, LastName, Title, TitleOfCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, HomePhone, Extension, Notes, ReportsTo, PhotoPath) 
            VALUES 
            (@FirstName, @LastName, @Title, @TitleOfCourtesy, @BirthDate, @HireDate, @Address, @City, @Region, @PostalCode, @Country, @HomePhone, @Extension, @Notes, @ReportsTo, @PhotoPath);
        ";
#pragma warning disable CA1062, CS8604
        AddParameter(command, "@FirstName", employee.FirstName);
        AddParameter(command, "@LastName", employee.LastName);
        AddParameter(command, "@Title", employee.Title);
        AddParameter(command, "@TitleOfCourtesy", employee.TitleOfCourtesy);
        AddParameter(command, "@BirthDate", employee.BirthDate);
        AddParameter(command, "@HireDate", employee.HireDate);
        AddParameter(command, "@Address", employee.Address);
        AddParameter(command, "@City", employee.City);
        AddParameter(command, "@Region", employee.Region);
        AddParameter(command, "@PostalCode", employee.PostalCode);
        AddParameter(command, "@Country", employee.Country);
        AddParameter(command, "@HomePhone", employee.HomePhone);
        AddParameter(command, "@Extension", employee.Extension);
        AddParameter(command, "@Notes", employee.Notes);
        AddParameter(command, "@ReportsTo", employee.ReportsTo);
        AddParameter(command, "@PhotoPath", employee.PhotoPath);
#pragma warning restore CA1062, CS8604
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception)
        {
            throw new EmployeeServiceException("Inserting an employee failed.");
        }

        return employee.Id;
    }

    /// <summary>
    /// Removes an employee from the the Employee table of the database based on the provided employee ID.
    /// </summary>
    /// <param name="employeeId">The ID of the employee to remove.</param>
    /// <exception cref="EmployeeServiceException"> Thrown when an error occurs while attempting to remove the employee.</exception>
    public void RemoveEmployee(long employeeId)
    {
        using var connection = _dbFactory.CreateConnection() ?? throw new ArgumentException("connection is null");
        connection.ConnectionString = _connectionString;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            @"
                DELETE FROM Employees WHERE EmployeeID = @EmployeeID;
            ";

        AddParameter(command, "@EmployeeID", employeeId);
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception)
        {
            throw new EmployeeServiceException();
        }
    }

    /// <summary>
    /// Updates an employee record in the Employee table of the database.
    /// </summary>
    /// <param name="employee">The employee object containing updated information.</param>
    /// <exception cref="EmployeeServiceException">Thrown when there is an issue updating the employee record.</exception>
    public void UpdateEmployee(Employee employee)
    {
        using var connection = _dbFactory.CreateConnection() ?? throw new ArgumentException("connection is null");
        connection.ConnectionString = _connectionString;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Employees
            SET 
                EmployeeID = @EmployeeID,
                FirstName = @FirstName,
                LastName = @LastName,
                Title = @Title,
                TitleOfCourtesy = @TitleOfCourtesy,
                BirthDate = @BirthDate,
                HireDate = @HireDate,
                Address = @Address,
                City = @City,
                Region = @Region,
                PostalCode = @PostalCode,
                Country = @Country,
                HomePhone = @HomePhone,
                Extension = @Extension,
                Notes = @Notes,
                ReportsTo = @ReportsTo,
                PhotoPath = @PhotoPath
            WHERE EmployeeID = @EmployeeID;
        ";
#pragma warning disable CS8604, CA1062
        AddParameter(command, "@EmployeeID", employee.Id);
        AddParameter(command, "@FirstName", employee.FirstName);
        AddParameter(command, "@LastName", employee.LastName);
        AddParameter(command, "@Title", employee.Title);
        AddParameter(command, "@TitleOfCourtesy", employee.TitleOfCourtesy);
        AddParameter(command, "@BirthDate", employee.BirthDate);
        AddParameter(command, "@HireDate", employee.HireDate);
        AddParameter(command, "@Address", employee.Address);
        AddParameter(command, "@City", employee.City);
        AddParameter(command, "@Region", employee.Region);
        AddParameter(command, "@PostalCode", employee.PostalCode);
        AddParameter(command, "@Country", employee.Country);
        AddParameter(command, "@HomePhone", employee.HomePhone);
        AddParameter(command, "@Extension", employee.Extension);
        AddParameter(command, "@Notes", employee.Notes);
        AddParameter(command, "@ReportsTo", employee.ReportsTo);
        AddParameter(command, "@PhotoPath", employee.PhotoPath);
#pragma warning restore CS8604

        try
        {
            int rowsEffected = command.ExecuteNonQuery();
            if (rowsEffected == 0)
            {
                throw new EmployeeServiceException("Employees is not updated.");
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static void AddParameter(DbCommand command, string parameterName, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}