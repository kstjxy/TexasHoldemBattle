local M = {}

M.name = 'test.01',
M.myaction = -1;

function M.start = function(gamerule)    --��Ϸ�ʼ(��ʼ�����ȴ������Ӧ)  
    print( M[name] + "��ʼ���ɹ���") 
    return M[name]
end

function M.round_start(gamestat)    --ÿ��������ʼ(������ÿ����ҿ���ʱ���)
    print( M[name] + "���֣�") 
    return
end

function M.action = function(gamestat)    --ÿ���֡����ö���(������ϸ��Ϣ���ȴ���Ҳ���)
    math.randomseed(tostring(os.time()):reverse():sub(1, 7))
    randNum = math.random(100)
    if (ranNum <= 40) then
        M.myaction = 1
        return M.myaction
    elseif (ranNum <= 45) then
        M.myaction = 2
        return M.myaction
    elseif (ranNum <= 50) then
        M.myaction = 3
        return M.myaction
    end
    M.myaction = 4
    return M.myaction
end

function M.finalCards = function(gamestat)    --ÿ���֡����ö���(������ϸ��Ϣ���ȴ���Ҳ���)
    math.randomseed(tostring(os.time()):reverse():sub(1, 7))
    first = math.random(10000) % 3
    if (first == 0) then
        second = math.random(10000) % 4 
        third = second + 1
    else
        second = 2*first
        third = 
    end
    third = second +
    
    M.myaction = 4
    return M.myaction
end

return M