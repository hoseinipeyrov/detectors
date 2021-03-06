﻿using System.Linq;
using Detectors.Kafka.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Detectors.Kafka.Controllers
{
    [Route("kafka")]
    public class KafkaHomeController : Controller
    {
        private readonly KafkaClusterConfigCollection _configuration;
        public KafkaHomeController(KafkaClusterConfigCollection configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("")]
        public IActionResult GetHomepage()
        {
            return Redirect("kafka/clusters");
        }
        
        [HttpGet("clusters")]
        [HttpGet("clusters.{format}")]
        public IActionResult GetClusterList()
        {
            return Ok(_configuration.GetAllKafkaClusterConfigs().Select(c => new {c.Id}));
        }
    }
}