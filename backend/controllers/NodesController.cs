// Modify the GetNodes method
[HttpGet]
public async Task<ActionResult<object>> GetNodes()
{
    var nodes = await _context.Nodes.ToListAsync();
    // Return in a format Unity's JsonUtility can parse
    return new { nodes = nodes };
}