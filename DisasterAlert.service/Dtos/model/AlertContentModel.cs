using System;
using DisasterAlert.context.Entities;

namespace DisasterAlert.service.Dtos.model;

public class AlertContentModel
{
    public RiskAlert Alert {get; set;}
    public string Text = string.Empty;
    public string Html = string.Empty;
}
