using Application.Operation.Features.Employee.CandidateUsers.Services;
using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Export;
using Application.Operation.Features.Employee.Dashboard.Services.Read;
using Application.Operation.Features.Employee.Dashboard.Services.Scopes;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Services;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Commands.Validators;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Services;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tawtheef.Application;

namespace Application.Operation
{
     /// <summary>
    /// Extension methods to register Application layer services (Mapster, Mediator, Validators, etc).
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplicationOperation(IConfiguration config)
            {
                services.AddApplicationLayer(config);
                services.RegisterMapster();
                services.RegisterMediator();
                services.RegisterFeatureServices();
                services.RegisterValidators();
                return services;
            }

            private void RegisterMapster()
            {
                var config = TypeAdapterConfig.GlobalSettings;
                config.Scan(typeof(OperationAssemblyMarker).Assembly);
            }

            private void RegisterMediator()
            {
                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
                    cfg.RegisterServicesFromAssembly(typeof(OperationAssemblyMarker).Assembly);
                    cfg.AddOpenBehavior(typeof(Tawtheef.Application.Common.Behaviours.ValidationBehaviour<,>));
                });
            }
            
            private void RegisterFeatureServices()
            {
                services.RegisterEmployeeCommonServices();
                services.RegisterDashboardServices();
                services.RegisterCandidateUsersServices();
                services.RegisterJobManagementServices();
                services.RegisterJobInvitationSummaryServices();
            }

            private void RegisterEmployeeCommonServices()
            {
                services.AddScoped<EmployeeProfileAccessContextProvider>();
                services.AddScoped<EmployeeProfileAccessScope>();
                services.AddScoped<EmployeeJobAccessContextProvider>();
            }

            private void RegisterDashboardServices()
            {
                services.AddScoped<DashboardAccessContextProvider>();
                services.AddScoped<DashboardQueryScope>();
                services.AddScoped<DashboardOverviewReader>();
                services.AddScoped<DashboardProfileMetricsReader>();
                services.AddScoped<DashboardJobMetricsReader>();
                services.AddScoped<DashboardInvitationMetricsReader>();
                services.AddScoped<DashboardWorkloadMetricsReader>();
                services.AddScoped<DashboardJobsReader>();
                services.AddScoped<DashboardInvitationsReader>();
                services.AddScoped<DashboardEmployeesReader>();
                services.AddScoped<DashboardSummaryExcelExporter>();
                services.AddScoped<DashboardCandidatesExcelExporter>();
                services.AddScoped<DashboardJobsExcelExporter>();
                services.AddScoped<DashboardEmployeesExcelExporter>();
                services.AddScoped<DashboardInvitationsExcelExporter>();
            }

            private void RegisterCandidateUsersServices()
            {
                services.AddScoped<CandidateUsersQueryBuilder>();
                services.AddScoped<CandidateUsersExcelExporter>();
            }

            private void RegisterJobManagementServices()
            {
                services.AddScoped<IJobTargetCandidateCalculatorService, JobTargetCandidateCalculatorService>();
                services.AddScoped<IJobRequirementsService, JobRequirementsService>();
                services.AddScoped<IJobCandidatesQueryBuilderService, JobCandidatesQueryBuilderService>();
                services.AddScoped<ICandidateEligibilityEvaluationService, CandidateEligibilityEvaluationService>();
                services.AddScoped<JobsExcelExporter>();
            }

            private void RegisterJobInvitationSummaryServices()
            {
                services.AddScoped<JobInvitationSummaryQueryBuilder>();
                services.AddScoped<JobInvitationSummaryExcelExporter>();
            }

            private void RegisterValidators()
            {
                services.AddValidatorsFromAssembly(typeof(CreateJobFromPreviousCommandValidator).Assembly);
                services.AddValidatorsFromAssembly(typeof(CreateJobCommandValidator).Assembly);
            }
        }
    }
}
