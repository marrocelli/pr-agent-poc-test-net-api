
import asyncio
from typing import Dict, Any
from openai import AsyncAzureOpenAI
from .tools.github import GitHubTool
#from .tools.azure import AzureTool
from .config.settings import settings

class Agent:
  def __init__(self, settings):
    self.settings = settings
    self.client = AsyncAzureOpenAI(
      azure_endpoint=settings.azure_openai_endpoint,
      api_key=settings.azure_openai_api_key,
      api_version="2024-02-01"
    )
    self.github = GitHubTool(settings.github_token, settings.github_repo)
    #self.azure = AzureTool(settings)
    
  async def run(self, command: Dict[str, Any]) -> Dict[str, Any]:
    """Main agent loop"""
    repo = command.get("repo")
    pr_number = command.get("pr_number")
    action = command.get("action")
    
    print(f"Agent processing: repo={repo}, pr={pr_number}, action={action}")
    
    # Step 1: Get PR details
    pr_info = await self.github.get_pr_metadata(repo, pr_number)
    
    # Step 2: Simple decision tree (LLM planning later)
    if action == "opened":
      summary = await self._handle_pr_opened(pr_info)
    elif action == "synchronize":
      summary = await self._handle_pr_updated(pr_info)
    else:
      summary = f"PR {pr_number} action '{action}' - no processing"
    
    # Step 3: Post comment
    await self.github.post_pr_comment(repo, pr_number, summary)
    
    return {
      "status": "completed",
      "pr_number": pr_number,
      "summary": summary[:500]  # Truncate for response
    }
    
  async def _handle_pr_opened(self, pr_info):
    """Handle new PR"""
    return f"🤖 PR Agent: New PR detected!\n\n" \
            f"**Title:** {pr_info['title']}\n" \
            f"**Author:** {pr_info['author']}\n" \
            f"**Analysis starting...**"
  
  async def _handle_pr_updated(self, pr_info):
    """Handle PR updates"""
    return f"🔄 PR Agent: PR updated!\n\n" \
            f"**Title:** {pr_info['title']}\n" \
            f"New commits detected. Re-analysis in progress..."
