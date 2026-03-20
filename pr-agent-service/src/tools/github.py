
import httpx
from typing import Dict, Any

class GitHubTool:
  def __init__(self, token: str, default_repo: str):
    self.token = token
    self.default_repo = default_repo
    self.headers = {
      "Authorization": f"token {token}",
      "Accept": "application/vnd.github.v3+json"
    }
    
  async def get_pr_metadata(self, repo_full_name: str, pr_number: int) -> Dict[str, Any]:
    """Get PR details"""
    owner, repo = repo_full_name.split("/")
    url = f"https://api.github.com/repos/{owner}/{repo}/pulls/{pr_number}"
    
    async with httpx.AsyncClient() as client:
      resp = await client.get(url, headers=self.headers)
      resp.raise_for_status()
      data = resp.json()
      
      return {
        "title": data["title"],
        "author": data["user"]["login"],
        "base_ref": data["base"]["ref"],
        "head_ref": data["head"]["ref"],
        "url": data["html_url"]
      }
  
  async def post_pr_comment(self, repo_full_name: str, pr_number: int, comment: str):
    """Post comment to PR"""
    owner, repo = repo_full_name.split("/")
    url = f"https://api.github.com/repos/{owner}/{repo}/issues/{pr_number}/comments"
    
    async with httpx.AsyncClient() as client:
      resp = await client.post(url, headers=self.headers, json={"body": comment})
      resp.raise_for_status()