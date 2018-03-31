﻿using System;
using System.Linq;
using Detectors.Kafka.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Detectors.Kafka.Controllers
{
    [Route("kafka/cluster/{clusterId}")]
    public class KafkaClusterController : Controller
    {
        private readonly KafkaClusterConfigCollection _configuration;
        public KafkaClusterController(KafkaClusterConfigCollection configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("")]
        public IActionResult GetClusterInfo(string clusterId)
        {
            return Redirect($"{clusterId}/metadata");
        }

        [HttpGet("metadata")]
        [HttpGet("metadata.{format}")]
        public IActionResult GetClusterMetadata(string clusterId)
        {
            using (var producer = _configuration.BuildProducer(clusterId))
            {
                if (producer == null)
                    return NotFound();

                var md = producer.GetMetadata(true, null, TimeSpan.FromSeconds(10));
                return Ok(md);
            }
        }
        
        [HttpGet("topics")]
        [HttpGet("topics.{format}")]
        public IActionResult GetTopicList(string clusterId)
        {
            using (var producer = _configuration.BuildProducer(clusterId))
            {
                if (producer == null)
                    return NotFound();
                
                var md = producer.GetMetadata(true, null, TimeSpan.FromSeconds(10));
                var resultObject = md.Topics.Select(t => new
                {
                    t.Topic,
                    PartitionCount = t.Partitions.Count
                }).ToList();
                return Ok(resultObject);
            }
        }
        
        [HttpGet("groups")]
        [HttpGet("groups.{format}")]
        public IActionResult GetGroupList(string clusterId)
        {
            using (var producer = _configuration.BuildProducer(clusterId))
            {
                if (producer == null)
                    return NotFound();

                // Dummy calls to GetMetadata to avoid "Broker transport failure" issue
                producer.GetMetadata();
                producer.GetMetadata();
                
                var groups = producer.ListGroups(TimeSpan.FromSeconds(10));
                var resultObject = groups.Select(g => new
                {
                    g.Group,
                    g.State,
                    g.Protocol,
                    g.ProtocolType,
                    MemberCount = g.Members.Count
                }).ToList();

                return Ok(resultObject);
            }
        }
    }
}