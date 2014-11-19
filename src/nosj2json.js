function nosj2json(nosj) {
    if (!nosj.length || typeof nosj[0] !== 'object' || !('length' in nosj[0])) {
        return nosj;
    }
    var result = [];
    var head = nosj[0];
    for (var i = 1; i < nosj.length; ++i) {
        var obj = {};
        for (var j = 0; j < nosj[i].length && j < head.length; ++j) {
            if (nosj[i][j] !== '__UNDEF__') {
                obj[head[j]] = nosj[i][j];
            }
        }
        result.push(obj);
    }
    return result;
}