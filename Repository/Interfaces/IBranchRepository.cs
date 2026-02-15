using CarDealer.LeadAutomation.Repository.DTOs;

namespace CarDealer.LeadAutomation.Repository.Interfaces;

public interface IBranchRepository
{
    BranchDTO GetBranchById(int branchId);
}